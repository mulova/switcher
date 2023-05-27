//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEditor;
    using UnityEngine;
    [CustomPropertyDrawer(typeof(RectTransformData), true)]
    public class RectTransformDataDrawer : CompDataDrawer { }

    [CustomPropertyDrawer(typeof(CompData), true)]
    public class CompDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Rect drawRect = rect;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            var data = property.GetValue() as CompData;
            foreach (var m in data.ListAttributedMembers())
            {
                if (m.HasChanged(data))
                {
                    var p = property.FindPropertyRelative(m.name);
                    EditorGUI.PropertyField(drawRect, p);
                    drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int count = 0;
            var data = property.GetValue() as CompData;
            foreach (var m in data.ListAttributedMembers())
            {
                if (m.HasChanged(data))
                {
                    count++;
                }
            }
            return count * EditorGUIUtility.singleLineHeight + Mathf.Max(count-1, 0) * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}