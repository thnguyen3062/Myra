using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GIKCore.Bundle
{
    [CustomPropertyDrawer(typeof(BundleSelectorAttribute))]
    public class BundleSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
                BundleSelectorAttribute attr = this.attribute as BundleSelectorAttribute;

                if (attr.UseDefaultDrawer)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    //generate the options
                    List<string> options = new List<string>() { "None" };
                    options.AddRange(AssetDatabase.GetAllAssetBundleNames());//get all asset bundle name and set to custom option

                    string propertyString = property.stringValue;
                    int index = -1;

                    if (string.IsNullOrEmpty(propertyString))
                    {//The options is empty
                        index = 0;//first index is the special <None> entry
                    }
                    else
                    {
                        //check if there is an entry that matches the entry and get the index
                        //we skip index 0 as that is a special <None> custom case
                        for (int i = 1; i < options.Count; i++)
                        {
                            if (options[i].Equals(propertyString))
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    //Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, options.ToArray());
                    //Adjust the actual string value of the property based on the selection
                    property.stringValue = (index >= 1) ? options[index] : "";
                }
                EditorGUI.EndProperty();
            }
            else EditorGUI.PropertyField(position, property, label);
        }
    }
}