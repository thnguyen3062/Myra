using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UIEngine.Extensions.Attribute;

namespace UIEngine.Extensions.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer
    {
		private bool _toShow = true;


		/// <summary>
		/// Key is Associated with drawer type (the T in [CustomPropertyDrawer(typeof(T))])
		/// Value is PropertyDrawer Type
		/// </summary>
		private static Dictionary<System.Type, System.Type> _allPropertyDrawersInDomain;


		private bool _initialized;
		private PropertyDrawer _customAttributeDrawer;
		private PropertyDrawer _customTypeDrawer;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!(attribute is ConditionalFieldAttribute conditional)) return 0;

			Initialize(property);

			SerializedProperty propertyToCheck = ConditionalFieldUtility.FindRelativeProperty(property, conditional.FieldToCheck);
			_toShow = ConditionalFieldUtility.PropertyIsVisible(propertyToCheck, conditional.Inverse, conditional.CompareValues);
			if (!_toShow) return 0;

			if (_customAttributeDrawer != null) return _customAttributeDrawer.GetPropertyHeight(property, label);
			if (_customTypeDrawer != null) return _customTypeDrawer.GetPropertyHeight(property, label);

			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_toShow) return;

			if (_customAttributeDrawer != null) TryUseAttributeDrawer();
			else if (_customTypeDrawer != null) TryUseTypeDrawer();
			else EditorGUI.PropertyField(position, property, label, true);


			void TryUseAttributeDrawer()
			{
				try
				{
					_customAttributeDrawer.OnGUI(position, property, label);
				}
				catch (System.Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to use Custom Attribute Drawer " + _customAttributeDrawer.GetType() + " : " + e, property);
				}
			}

			void TryUseTypeDrawer()
			{
				try
				{
					_customTypeDrawer.OnGUI(position, property, label);
				}
				catch (System.Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to instantiate " + fieldInfo.FieldType + " : " + e, property);
				}
			}
		}


		private void Initialize(SerializedProperty property)
		{
			if (_initialized) return;

			CacheAllDrawersInDomain();

			TryGetCustomAttributeDrawer();
			TryGetCustomTypeDrawer();

			_initialized = true;


			void CacheAllDrawersInDomain()
			{
				if (_allPropertyDrawersInDomain != null && _allPropertyDrawersInDomain.Count > 0) return;

				_allPropertyDrawersInDomain = new Dictionary<System.Type, System.Type>();
				System.Type propertyDrawerType = typeof(PropertyDrawer);

				IEnumerable<System.Type> allDrawerTypesInDomain = System.AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.Where(t => propertyDrawerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

				foreach (System.Type type in allDrawerTypesInDomain)
				{
					CustomAttributeData drawerAttribute = CustomAttributeData.GetCustomAttributes(type).FirstOrDefault();
					if (drawerAttribute == null) continue;
					System.Type associatedType = drawerAttribute.ConstructorArguments.FirstOrDefault().Value as System.Type;
					if (associatedType == null) continue;

					if (_allPropertyDrawersInDomain.ContainsKey(associatedType)) continue;
					_allPropertyDrawersInDomain.Add(associatedType, type);
				}
			}

			void TryGetCustomAttributeDrawer()
			{
				if (fieldInfo == null) return;
				//Get the second attribute flag
				PropertyAttribute secondAttribute = (PropertyAttribute)fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
					.FirstOrDefault(a => !(a is ConditionalFieldAttribute));
				if (secondAttribute == null) return;
				System.Type genericAttributeType = secondAttribute.GetType();

				//Get the associated attribute drawer
				if (!_allPropertyDrawersInDomain.ContainsKey(genericAttributeType)) return;

				System.Type customAttributeDrawerType = _allPropertyDrawersInDomain[genericAttributeType];
				CustomAttributeData customAttributeData = fieldInfo.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType == secondAttribute.GetType());
				if (customAttributeData == null) return;


				//Create drawer for custom attribute
				try
				{
					_customAttributeDrawer = (PropertyDrawer)System.Activator.CreateInstance(customAttributeDrawerType);
					FieldInfo attributeField = customAttributeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
					if (attributeField != null) attributeField.SetValue(_customAttributeDrawer, secondAttribute);
				}
				catch (System.Exception e)
				{
					LogWarning("Unable to construct drawer for " + secondAttribute.GetType() + " : " + e, property);
				}
			}

			void TryGetCustomTypeDrawer()
			{
				if (fieldInfo == null) return;
				// Skip checks for mscorlib.dll
				if (fieldInfo.FieldType.Module.ScopeName.Equals(typeof(int).Module.ScopeName)) return;


				// Of all property drawers in the assembly we need to find one that affects target type
				// or one of the base types of target type
				System.Type fieldDrawerType = null;
				System.Type fieldType = fieldInfo.FieldType;
				while (fieldType != null)
				{
					if (_allPropertyDrawersInDomain.ContainsKey(fieldType))
					{
						fieldDrawerType = _allPropertyDrawersInDomain[fieldType];
						break;
					}

					fieldType = fieldType.BaseType;
				}

				if (fieldDrawerType == null) return;

				//Create instances of each (including the arguments)
				try
				{
					_customTypeDrawer = (PropertyDrawer)System.Activator.CreateInstance(fieldDrawerType);
				}
				catch (System.Exception e)
				{
					LogWarning("No constructor available in " + fieldType + " : " + e, property);
					return;
				}

				//Reassign the attribute field in the drawer so it can access the argument values
				FieldInfo attributeField = fieldDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
				if (attributeField != null) attributeField.SetValue(_customTypeDrawer, attribute);
				FieldInfo fieldInfoField = fieldDrawerType.GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);
				if (fieldInfoField != null) fieldInfoField.SetValue(_customTypeDrawer, fieldInfo);
			}
		}

		private void LogWarning(string log, SerializedProperty property)
		{
			string warning = "Property <color=brown>" + fieldInfo.Name + "</color>";
			if (fieldInfo != null && fieldInfo.DeclaringType != null)
				warning += " on behaviour <color=brown>" + fieldInfo.DeclaringType.Name + "</color>";
			warning += " caused: " + log;

			Debug.LogWarning(warning, property.serializedObject.targetObject);
		}
	}

	public static class ConditionalFieldUtility
	{
		#region Property Is Visible

		public static bool PropertyIsVisible(SerializedProperty property, bool inverse, string[] compareAgainst)
		{
			if (property == null) return true;

			string asString =  AsStringValue(property).ToUpper();

			if (compareAgainst != null && compareAgainst.Length > 0)
			{
				bool matchAny = CompareAgainstValues(asString, compareAgainst, IsFlagsEnum());
				if (inverse) matchAny = !matchAny;
				return matchAny;
			}

			bool someValueAssigned = asString != "FALSE" && asString != "0" && asString != "NULL";
			if (someValueAssigned) return !inverse;

			return inverse;


			bool IsFlagsEnum()
			{
				if (property.propertyType != SerializedPropertyType.Enum) return false;				
				return property.GetType().GetCustomAttribute<System.FlagsAttribute>() != null;
			}
		}

		private static string AsStringValue(SerializedProperty prop)
		{
			switch (prop.propertyType)
			{
				case SerializedPropertyType.String:
					return prop.stringValue;

				case SerializedPropertyType.Character:
				case SerializedPropertyType.Integer:
					if (prop.type == "char") return System.Convert.ToChar(prop.intValue).ToString();
					return prop.intValue.ToString();

				case SerializedPropertyType.ObjectReference:
					return prop.objectReferenceValue != null ? prop.objectReferenceValue.ToString() : "null";

				case SerializedPropertyType.Boolean:
					return prop.boolValue.ToString();

				case SerializedPropertyType.Enum:
					return prop.enumNames[prop.enumValueIndex];

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// True if the property value matches any of the values in '_compareValues'
		/// </summary>
		private static bool CompareAgainstValues(string propertyValueAsString, string[] compareAgainst, bool handleFlags)
		{
			if (!handleFlags) return ValueMatches(propertyValueAsString);

			string[] separateFlags = propertyValueAsString.Split(',');
			foreach (string flag in separateFlags)
			{
				if (ValueMatches(flag.Trim())) return true;
			}

			return false;


			bool ValueMatches(string value)
			{
				foreach (string compare in compareAgainst) if (value == compare) return true;
				return false;
			}
		}

		#endregion


		#region Find Relative Property

		public static SerializedProperty FindRelativeProperty(SerializedProperty property, string propertyName)
		{
			if (property.depth == 0) return property.serializedObject.FindProperty(propertyName);

			string path = property.propertyPath.Replace(".Array.data[", "[");
			string[] elements = path.Split('.');

			SerializedProperty nestedProperty = NestedPropertyOrigin(property, elements);

			// if nested property is null = we hit an array property
			if (nestedProperty == null)
			{
				string cleanPath = path.Substring(0, path.IndexOf('['));
				SerializedProperty arrayProp = property.serializedObject.FindProperty(cleanPath);
				Object target = arrayProp.serializedObject.targetObject;

				string who = "Property <color=brown>" + arrayProp.name + "</color> in object <color=brown>" + target.name + "</color> caused: ";
				string warning = who + "Array fields is not supported by [ConditionalFieldAttribute]. Consider to use <color=blue>CollectionWrapper</color>";

				Debug.LogWarning(warning, target);

				return null;
			}

			return nestedProperty.FindPropertyRelative(propertyName);
		}

		// For [Serialized] types with [Conditional] fields
		private static SerializedProperty NestedPropertyOrigin(SerializedProperty property, string[] elements)
		{
			SerializedProperty parent = null;

			for (int i = 0; i < elements.Length - 1; i++)
			{
				string element = elements[i];
				int index = -1;
				if (element.Contains("["))
				{
					index = System.Convert.ToInt32(element.Substring(element.IndexOf("[", System.StringComparison.Ordinal))
						.Replace("[", "").Replace("]", ""));
					element = element.Substring(0, element.IndexOf("[", System.StringComparison.Ordinal));
				}

				parent = i == 0
					? property.serializedObject.FindProperty(element)
					: parent != null
						? parent.FindPropertyRelative(element)
						: null;

				if (index >= 0 && parent != null) parent = parent.GetArrayElementAtIndex(index);
			}

			return parent;
		}

		#endregion

		#region Behaviour Property Is Visible

		public static bool BehaviourPropertyIsVisible(UnityEngine.Object obj, string propertyName, ConditionalFieldAttribute appliedAttribute)
		{
			if (string.IsNullOrEmpty(appliedAttribute.FieldToCheck)) return true;

			SerializedObject so = new SerializedObject(obj);
			SerializedProperty property = so.FindProperty(propertyName);
			SerializedProperty targetProperty = FindRelativeProperty(property, appliedAttribute.FieldToCheck);

			return PropertyIsVisible(targetProperty, appliedAttribute.Inverse, appliedAttribute.CompareValues);
		}

		#endregion
	}
}
