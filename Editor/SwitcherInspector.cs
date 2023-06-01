﻿//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Switcher)), CanEditMultipleObjects]
    public class SwitcherInspector : Editor
    {
        private Switcher uiSwitch;
        internal static bool exclusive = true;
        internal static HashSet<string> activeSet = new HashSet<string>();
        private bool hasAction;
        private bool hasPreset;
        internal static bool showAction { get; set; } = false;
        internal static bool showData { get; set; } = false;
        internal static bool showPreset { get; set; } = false;

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

        private void OnEnable()
        {
            uiSwitch = (Switcher)target;

            hasAction = uiSwitch.switches.Find(s => s.action != null && s.action.GetPersistentEventCount() > 0) != null;
            hasPreset = uiSwitch.preset.Count > 0;
            showAction = false;
            showData = false;
            showPreset = false;
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
                    if (uiSwitch.preset.Count > 0)
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label("Preset");
                            foreach (var p in uiSwitch.preset)
                            {
                                using (new ColorScope(Color.green, IsPreset(p.keys)))
                                {
                                    if (GUILayout.Button(p.presetName, GUILayout.MaxWidth(200)))
                                    {
                                        uiSwitch.SetPreset(p.presetName);
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
                    foreach (var s in uiSwitch.switches)
                    {
                        using (new ColorScope(Color.green, IsActive(s.name)))
                        {
                            if (GUILayout.Button(s.name, GUILayout.MaxWidth(200)))
                            {
                                uiSwitch.Apply(s.name);
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
            DrawDefaultInspector();
            if (uiSwitch.switches.Count == 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Separator();

                if (!string.IsNullOrEmpty(createSwitcherErr))
                {
                    EditorGUILayout.HelpBox(createSwitcherErr, MessageType.Error);
                }
            } else if (uiSwitch.switches.Count > 0)
            {
                if (showPreset)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("preset"), true);
                }
                EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (hasAction && !showAction)
                    {
                        if (GUILayout.Button("Actions"))
                        {
                            showAction = true;
                        }
                    }
                    if (GUILayout.Button("Rename"))
                    {
                        SwitchSetDrawer.rename = !SwitchSetDrawer.rename;
                    }
                    if (GUILayout.Button("Data"))
                    {
                        showData = !showData;
                    }
                    if (hasPreset && !showPreset)
                    {
                        if (GUILayout.Button("Preset"))
                        {
                            showPreset = true;
                        }
                    }
                }
                if (GUILayout.Button("Open Compare View"))
                {
                    SwitcherCompareWindow.Get();
                }
            }
        }
    }
}