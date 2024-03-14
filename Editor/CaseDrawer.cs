//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace mulova.switcher
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(Case))]
    public class CaseDrawer : PropertyDrawerBase
    {
        public static readonly Color SelectedColor = Color.green;

        private Object[] GetAllTargets(SerializedProperty p)
        {
            var switcher = p.serializedObject.targetObject as Switcher;
            HashSet<Object> targets = new HashSet<Object>();
            foreach (var d in switcher.cases[0].data)
            {
                if (d?.target != null)
                {
                    targets.Add(d.target.gameObject);
                }
            }
            return new List<Object>(targets).ToArray();
        }

        internal static bool rename = false;
        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            var switcher = p.serializedObject.targetObject as Switcher;
            // Draw Name
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            var active = SwitcherInspector.IsActive(n.stringValue);
            var nameBounds = bounds[0].SplitByWidthsRatio(0.75f, 0.25f);
            var boundsLeft = bounds[1];
            // indentation
            boundsLeft.x += 30;
            boundsLeft.width -= 30;
            Color c = GUI.color;
            if (active)
            {
                c = SelectedColor;
            }

            // Draw Title
            if (rename || !IsValidId(n))
            {
                EditorGUI.PropertyField(nameBounds[0], n, new GUIContent(""));
            } else
            {
                using (new EnableScope(!string.IsNullOrEmpty(n.stringValue)))
                {
                    using (new ColorScope(c))
                    {
                        if (GUI.Button(nameBounds[0], new GUIContent(n.stringValue)))
                        {
                            Undo.RecordObjects(GetAllTargets(p), "switch");
                            bool hasPreset = switcher.preset.Count > 0;
                            if (!hasPreset)
                            {
                                SwitcherInspector.SetActive(n.stringValue, n.stringValue);
                            }
                            else
                            {
                                SwitcherInspector.Activate(n.stringValue, !SwitcherInspector.IsActive(n.stringValue));
                            }
                            var script = p.serializedObject.targetObject as Switcher;
                            script.Apply(n.stringValue);
                        }
                    }
                }
            }
            //GUI.enabled = !PrefabUtility.IsPartOfPrefabInstance(switcher); // Prevent information lost because collected CompData.target values are lost when the prefab is applied.
            if (EditorGUI.DropdownButton(nameBounds[1], new GUIContent("Options"), FocusType.Passive))
            {

                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Rename"), false, handleItemClicked, "Rename");
                menu.AddItem(new GUIContent("Collect"), false, handleItemClicked, "Collect");
                if (!switcher.hasAction)
                {
                    menu.AddItem(new GUIContent("Actions"), false, handleItemClicked, "Actions");
                }
                menu.DropDown(nameBounds[1]);

                void handleItemClicked(object o)
                {
                    var param = o as string;
                    switch (param)
                    {
                        case "Collect":
                            if (EditorUtility.DisplayDialog("Warning", "Collect and override data from variables/properties?", "Ok", "Cancel"))
                            {
                                var targets = GetAllTargets(p);
                                Undo.RecordObjects(targets, "Collect");
                                switcher.Collect(n.stringValue, true);
                                EditorUtil.SetDirty(switcher);
                            }
                            break;
                        case "Rename":
                            rename = !rename;
                            break;
                        case "Actions":
                            var c = switcher.GetCase(n.stringValue);
                            c.showAction = !c.showAction;
                            break;
                    }
                }
            }
            GUI.enabled = true;

            // Draw Actions
            if (switcher.showAction)
            {
                var actionBounds = boundsLeft.SplitByHeights(actionHeight);
                boundsLeft = actionBounds[1];
                var actionProperty = p.FindPropertyRelative("action");
                EditorGUI.PropertyField(actionBounds[0], actionProperty);
            }

            if (switcher.showData)
            {
                // Draw CompData
                var dataBounds = boundsLeft.SplitByHeights(dataHeight);
                boundsLeft = dataBounds[1];
                var dataProperty = p.FindPropertyRelative("data");
                EditorGUI.PropertyField(dataBounds[0], dataProperty, true);
            }

            bool IsValidId(SerializedProperty p)
            {
                var typeName = p.serializedObject.FindProperty("enumType").stringValue;
                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    var type = TypeEx.GetType(typeName);
                    if (type != null)
                    {
                        return Enum.TryParse(type, p.stringValue, out var _);
                    }
                }
                return true;
            }
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private int dataHeight;
        private int actionHeight;
        public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
        {
            var switcher = p.serializedObject.targetObject as Switcher;
            var separator = 0;
            float height = 0;

            dataHeight = switcher.showData ? (int)EditorGUI.GetPropertyHeight(p.FindPropertyRelative(nameof(Case.data))): 0;
            height += dataHeight;
            actionHeight = switcher.showAction ? (int)EditorGUI.GetPropertyHeight(p.FindPropertyRelative(nameof(Case.action))): 0;
            height += actionHeight;

            height += lineHeight + separator;
            return height;
        }
    }
}