//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.switcher
{
    [CustomPropertyDrawer(typeof(SwitchSet))]
    public class SwitchSetDrawer : PropertyDrawerBase
    {
        public static readonly Color SelectedColor = Color.green;

        private Object[] GetAllTargets(SerializedProperty p)
        {
            var switcher = p.serializedObject.targetObject as Switcher;
            HashSet<Object> targets = new HashSet<Object>();
            foreach (var d in switcher.switches[0].data)
            {
                //targets.Add(d.target);
                targets.Add(d.target.gameObject);
            }
            return new List<Object>(targets).ToArray();
        }

        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            var uiSwitch = p.serializedObject.targetObject as Switcher;
            // Draw Name
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            var active = SwitcherInspector.IsActive(n.stringValue);
            var nameBounds = bounds[0].SplitByWidthsRatio(0.5f, 0.25f, 0.25f);
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
            using (new EnableScope(!string.IsNullOrEmpty(n.stringValue)))
            {
                using (new ColorScope(c))
                {
                    if (GUI.Button(nameBounds[0], new GUIContent(n.stringValue)))
                    {
                        Undo.RecordObjects(GetAllTargets(p), "switch");
                        bool hasPreset = uiSwitch.preset.Count > 0;
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
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
            GUI.enabled = !PrefabUtility.IsPartOfPrefabInstance(uiSwitch); // Prevent information lost because collected ICompData.target values are lost when the prefab is applied.
            if (GUI.Button(nameBounds[2], "Collect"))
            {
                var targets = GetAllTargets(p);
                Undo.RecordObjects(targets, "Collect");
                //p.serializedObject.Update();
                uiSwitch.Collect(n.stringValue);
                EditorUtil.SetDirty(uiSwitch);
                //p.serializedObject.ApplyModifiedProperties();
                //PrefabUtility.RecordPrefabInstancePropertyModifications(uiSwitch);
            }
            GUI.enabled = true;

            // Draw Actions
            if (SwitcherInspector.showAction)
            {
                var actionBounds = boundsLeft.SplitByHeights(actionHeight);
                boundsLeft = actionBounds[1];
                var actionProperty = p.FindPropertyRelative("action");
                EditorGUI.PropertyField(actionBounds[0], actionProperty);
            }

            if (SwitcherInspector.showData)
            {
                // Draw ICompData
                var dataBounds = boundsLeft.SplitByHeights(dataHeight);
                boundsLeft = dataBounds[1];
#if UNITY_2019_1_OR_NEWER
                var dataProperty = p.FindPropertyRelative("data");
                EditorGUI.PropertyField(dataBounds[0], dataProperty, true);
#endif
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
            var uiSwitch = p.serializedObject.targetObject as Switcher;
            var separator = 0;
            float height = 0;

#if UNITY_2019_1_OR_NEWER
            dataHeight = SwitcherInspector.showData ? (int)EditorGUI.GetPropertyHeight(p.FindPropertyRelative("data")): 0;
            height += dataHeight;
#endif
            actionHeight = SwitcherInspector.showAction ? (int)EditorGUI.GetPropertyHeight(p.FindPropertyRelative("action")): 0;
            height += actionHeight;

            height += lineHeight + separator;
            return height;
        }
    }
}