//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace mulova.switcher
{
    using UnityEditor;
    using UnityEngine;
    [CustomPropertyDrawer(typeof(RectTransformData), true)]
    public class RectTransformDataDrawer : CompDataDrawer { }

    public static class MemberControlEx
    {
        public static bool IsUiActive(this MemberControl m, CompData c) => c.isActiveAndEnabled || (typeof(Transform).IsAssignableFrom(c.srcType) && m.name == "enabled");
    }

    [CustomPropertyDrawer(typeof(CompData), true)]
    public class CompDataDrawer : PropertyDrawer
    {
        private Type type;
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var drawRect = rect;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            var data = property.GetValue() as CompData;
            if (data != null)
            {
                foreach (var m in data.ListAttributedMembers())
                {
                    var enableScope = new EnableScope(m.IsUiActive(data));
                    if (m.HasChanged(data))
                    {
                        var p = property.FindPropertyRelative(m.name);
                        EditorGUI.PropertyField(drawRect, p);
                        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            } else
            {
                var s = property.serializedObject.targetObject as Switcher;
                var comps = s.gameObject.GetComponentsInChildren<Component>();
                var types = comps.Select(c => c.GetType()).Distinct().Where(t=> CompDataFactory.instance.FindDataType(t) != null).ToArray();
                Array.Sort(types, (a,b)=> a.FullName.CompareTo(b.FullName));
                if (type == null)
                {
                    type = types[0];
                }

                var btnWidth = 70;
                var popupRect = rect;
                var btnRect = rect;
                popupRect.width -= btnWidth;
                btnRect.x += popupRect.width;
                btnRect.width = btnWidth;
                EditorGUIEx.Popup(popupRect, "Type", ref type, types);
                if (GUI.Button(btnRect, "Add"))
                {
                    var regex = new Regex("cases\\.Array\\.data\\[(?<caseNo>[0-9]+)\\].data.Array.data\\[(?<dataNo>[0-9]+)\\]");
                    var match = regex.Match(property.propertyPath);
                    var dataIndex = int.Parse(match.Groups["dataNo"].Value);
                    for (int c = 0; c < s.cases.Count; ++c)
                    {
                        var path = SwitcherCompareTab.GetDataPropertyPath(c, dataIndex);
                        var prop = property.serializedObject.FindProperty(path);
                        var inst = CompDataFactory.instance.GetComponentData(type);
                        var m = inst.GetMember("enabled");
                        m.SetChanged(inst, true);
                        prop.SetValue(inst);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int count = 0;
            var data = property.GetValue() as CompData;
            if (data != null)
            {
                foreach (var m in data.ListAttributedMembers())
                {
                    if (m.HasChanged(data))
                    {
                        count++;
                    }
                }
            } else
            {
                count = 1;
            }
            return count * EditorGUIUtility.singleLineHeight + Mathf.Max(count-1, 0) * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}