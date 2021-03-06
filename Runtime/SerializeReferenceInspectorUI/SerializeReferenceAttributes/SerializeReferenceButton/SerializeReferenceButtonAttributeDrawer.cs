﻿#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER

using UnityEditor; 
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializeReferenceButtonAttribute))]
public class SerializeReferenceButtonAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property); 

        var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelPosition, label);    
         
        property.DrawSelectionButtonForManagedReference(position);
        
        EditorGUI.PropertyField(position, property, GUIContent.none, true);
        
        EditorGUI.EndProperty(); 
    } 
}
#endif

