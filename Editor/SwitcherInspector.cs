//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(Switcher)), CanEditMultipleObjects]
    public class SwitcherInspector : Editor
    {
        private Switcher switcher;
        internal static HashSet<string> activeSet = new HashSet<string>();
        private bool hasPreset;
        internal static bool showPreset { get; set; } = false;

        private SerializedProperty enumType;
        private SerializedProperty caseSensitive;

        private void OnEnable()
        {
            switcher = (Switcher)target;
            hasPreset = switcher.preset.Count > 0;
            showPreset = false;
            enumType = serializedObject.FindProperty("enumType");
            caseSensitive = serializedObject.FindProperty("caseSensitive");
            foreach (var c in switcher.cases)
            {
                if (c.IsApplied())
                {
                    activeSet.Add(c.name);
                }
            }
            CaseDrawer.rename = false;
        }

        internal static bool IsPreset(IList<string> actives)
        {
            if (activeSet.Count != actives.Count)
            {
                return false;
            }
            foreach (var a in actives)
            if (actives != null)
            {
                if (!activeSet.Contains(a))
                {
                    return false;
                }
            }
            return true;
        }

        internal static void SetActive(params string[] actives)
        {
            activeSet.Clear();
            if (actives != null)
            {
                activeSet.AddAll(actives);
            }
        }

        internal static void Activate(string id, bool active)
        {
            if (active)
            {
                activeSet.Add(id);
            } else
            {
                activeSet.Remove(id);
            }
        }

        internal static bool IsActive(string active)
        {
            return activeSet.Contains(active);
        }

        private void OnDisable()
        {
            activeSet.Clear();
        }

        private void OnSceneGUI()
        {
            Handles.BeginGUI();
            bool locked = GUILayout.Toggle(ActiveEditorTracker.sharedTracker.isLocked, "Lock");
            if (locked ^ ActiveEditorTracker.sharedTracker.isLocked)
            {
                ActiveEditorTracker.sharedTracker.isLocked = locked;
            }
            if (ActiveEditorTracker.sharedTracker.isLocked)
            {
                if (hasPreset)
                {
                    if (switcher.preset.Count > 0)
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Preset");
                            foreach (var p in switcher.preset)
                            {
                                using (new ColorScope(Color.green, IsPreset(p.keys)))
                                {
                                    if (GUILayout.Button(p.presetName, GUILayout.MaxWidth(200)))
                                    {
                                        switcher.SetPreset(p.presetName);
                                        SetActive(p.keys);
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.Space(10);
                }
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label("Option");
                    foreach (var s in switcher.cases)
                    {
                        using (new ColorScope(Color.green, IsActive(s.name)))
                        {
                            if (GUILayout.Button(s.name, GUILayout.MaxWidth(200)))
                            {
                                switcher.Apply(s.name);
                                SetActive(s.name);
                            }
                        }
                    }
                }
            }
            Handles.EndGUI();
        }

        private string createSwitcherErr;
        public override void OnInspectorGUI()
        {
            var isMultiple = Selection.gameObjects.Length > 1;

            using (var c = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultInspector();
                if (c.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    switcher.Reset();
                }
            }
            if (switcher.cases.Count == 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();

                if (!string.IsNullOrEmpty(createSwitcherErr))
                {
                    EditorGUILayout.HelpBox(createSwitcherErr, MessageType.Error);
                }
            } else if (switcher.cases.Count > 0 && !isMultiple)
            {
                if (GUILayout.Button("Open Compare View"))
                {
                    SwitcherCompareWindow.Get();
                }
                EditorGUILayout.Separator();
                if (showPreset)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("preset"), true);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (hasPreset && !showPreset)
                    {
                        if (GUILayout.Button("Preset"))
                        {
                            showPreset = true;
                        }
                    }
                }
            }

            var showMisc = switcher.showMisc || isMultiple;
            var showEnumType = !string.IsNullOrWhiteSpace(enumType.stringValue);
            if (showMisc || showEnumType)
            {
                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(enumType);
                    if (c.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                        CaseDrawer.rename = !string.IsNullOrWhiteSpace(enumType.stringValue);
                    }
                }
            }
            if (showMisc || !switcher.caseSensitive)
            {
                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(caseSensitive);
                    if (c.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}