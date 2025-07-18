using System;
using UnityEngine;
using UnityEditor;
using UIEngine.Extensions.Attribute;

namespace UIEngine.Extensions.Editor
{
    [CustomPropertyDrawer(typeof(HelpAttribute))]
    public class HelpDrawer : PropertyDrawer
    {
        // Used for top and bottom padding between the text and the HelpBox border.
        private const int paddingHeight = 8;

        // Used to add some margin between the the HelpBox and the property.
        private const int marginHeight = 2;

        //  Global field to store the original (base) property height.
        private float baseHeight = 0;

        // Custom added height for drawing text area which has the MultilineAttribute.
        private float addedHeight = 0;

        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a HelpAttribute.
        /// </summary>
        private HelpAttribute helpAttribute { get { return (HelpAttribute)attribute; } }

        /// <summary>
        /// A helper property to check for RangeAttribute.
        /// </summary>
        private RangeAttribute rangeAttribute
        {
            get
            {
                object[] attributes = fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true);
                return attributes != null && attributes.Length > 0 ? (RangeAttribute)attributes[0] : null;
            }
        }

        /// <summary>
        /// A helper property to check for MultiLineAttribute.
        /// </summary>
        private MultilineAttribute multilineAttribute
        {
            get
            {
                object[] attributes = fieldInfo.GetCustomAttributes(typeof(MultilineAttribute), true);
                return attributes != null && attributes.Length > 0 ? (MultilineAttribute)attributes[0] : null;
            }
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            // We store the original property height for later use...
            baseHeight = base.GetPropertyHeight(prop, label);

            // This stops icon shrinking if text content doesn't fill out the container enough.
            float minHeight = paddingHeight * 5;

            // Calculate the height of the HelpBox using the GUIStyle on the current skin and the inspector
            // window's currentViewWidth.
            GUIContent content = new GUIContent(helpAttribute.text);
            GUIStyle style = GUI.skin.GetStyle("helpbox");

            float height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);

            // We add tiny padding here to make sure the text is not overflowing the HelpBox from the top
            // and bottom.
            height += marginHeight * 2;

            // Since we draw a custom text area with the label above if our property contains the
            // MultilineAttribute, we need to add some extra height to compensate. This is stored in a
            // seperate global field so we can use it again later.
            if (multilineAttribute != null && prop.propertyType == SerializedPropertyType.String)
            {
                addedHeight = 48f;
            }

            // If the calculated HelpBox is less than our minimum height we use this to calculate the returned
            // height instead.
            return height > minHeight ? height + baseHeight + addedHeight : minHeight + baseHeight + addedHeight;
        }


        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // We get a local reference to the MultilineAttribute as we use it twice in this method and it
            // saves calling the logic twice for minimal optimization, etc...
            MultilineAttribute multiline = multilineAttribute;

            EditorGUI.BeginProperty(position, label, prop);

            // Copy the position out so we can calculate the position of our HelpBox without affecting the
            // original position.
            Rect helpPos = position;

            helpPos.height -= baseHeight + marginHeight;


            if (multiline != null)
            {
                helpPos.height -= addedHeight;
            }

            // Renders the HelpBox in the Unity inspector UI.
            EditorGUI.HelpBox(helpPos, helpAttribute.text, helpAttribute.type);

            position.y += helpPos.height + marginHeight;
            position.height = baseHeight;


            // If we have a RangeAttribute on our field, we need to handle the PropertyDrawer differently to
            // keep the same style as Unity's default.
            RangeAttribute range = rangeAttribute;

            if (range != null)
            {
                if (prop.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.Slider(position, prop, range.min, range.max, label);
                }
                else if (prop.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.IntSlider(position, prop, (int)range.min, (int)range.max, label);
                }
                else
                {
                    // Not numeric so draw standard property field as punishment for adding RangeAttribute to
                    // a property which can not have a range :P
                    EditorGUI.PropertyField(position, prop, label);
                }
            }
            else if (multiline != null)
            {
                // Here's where we handle the PropertyDrawer differently if we have a MultiLineAttribute, to try
                // and keep some kind of multiline text area. This is not identical to Unity's default but is
                // better than nothing...
                if (prop.propertyType == SerializedPropertyType.String)
                {
                    GUIStyle style = GUI.skin.label;
                    float size = style.CalcHeight(label, EditorGUIUtility.currentViewWidth);

                    EditorGUI.LabelField(position, label);

                    position.y += size;
                    position.height += addedHeight - size;

                    // Fixed text dissappearing thanks to: http://answers.unity3d.com/questions/244043/textarea-does-not-work-text-dissapears-solution-is.html
                    prop.stringValue = EditorGUI.TextArea(position, prop.stringValue);
                }
                else
                {
                    // Again with a MultilineAttribute on a non-text field deserves for the standard property field
                    // to be drawn as punishment :P
                    EditorGUI.PropertyField(position, prop, label);
                }
            }
            else
            {
                // If we get to here it means we're drawing the default property field below the HelpBox. More custom
                // and built in PropertyDrawers could be implemented to enable HelpBox but it could easily make for
                // hefty else/if block which would need refactoring!
                EditorGUI.PropertyField(position, prop, label);
            }

            EditorGUI.EndProperty();
        }
    }
}
