//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------


namespace mulova.switcher.foundation
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(EnumPopupAttribute))]
    public class EnumPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumPopupAttribute attr = attribute as EnumPopupAttribute;
            var typeVar = property.serializedObject.FindProperty(attr.enumTypeVar);
            if (typeVar != null)
            {
                var enumType = TypeEx.GetType(typeVar.stringValue);
                bool isEnum = false;
                if (enumType != null)
                {
                    try
                    {
                        Enum e1 = (Enum)enumType.GetEnumValues().GetValue(0);
                        if (!string.IsNullOrEmpty(property.stringValue))
                        {
                            e1 = (Enum)Enum.Parse(enumType, property.stringValue, true);
                        }
                        Enum e2 = EditorGUI.EnumPopup(position, label, e1);
                        if (e1 != e2)
                        {
                            property.stringValue = e2.ToString();
                        }
                        isEnum = true;
                    }
#pragma warning disable ERP022 // Unobserved exception in generic exception handler
                    catch
                    {
                    }
#pragma warning restore ERP022 // Unobserved exception in generic exception handler
                }
                if (!isEnum)
                {
                    var newStr = EditorGUI.TextField(position, label, property.stringValue);
                    if (newStr != property.stringValue)
                    {
                        property.stringValue = newStr;
                    }
                }
            } else
            {
                EditorGUI.HelpBox(position, "Invalid EnumType Variable Name: " + attr.enumTypeVar, MessageType.Error);
            }
        }
    }
}
