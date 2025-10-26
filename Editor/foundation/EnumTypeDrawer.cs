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
    [CustomPropertyDrawer(typeof(EnumTypeAttribute))]
    public class EnumTypeDrawer : PropertyDrawer
    {
        private TypeSelector typeSelector;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (typeSelector == null)
            {
                typeSelector = new TypeSelector(typeof(Enum));
            }
            typeSelector.DrawSelector(position, property);
        }
    }
}
