﻿//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEditor;
    using UnityEngine;

    public class SwitcherCompareTab : EditorTab
    {
        //private WindowToolbar toolbar = new WindowToolbar();
        internal SerializedObject so;
        internal Switcher switcher;
        private int setIndex;

        public SwitcherCompareTab(int index, TabbedEditorWindow window) : base(window)
        {
            this.setIndex = index;
        }

        public override void AddContextMenu() {}
        public override void OnTabChange(bool sel) { }
        public override void OnFocus(bool focus) { }

        public override void OnSelectionChange()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            var s = Selection.activeGameObject.GetComponent<Switcher>();
            if (s != null)
            {
                switcher = s;
                so = new SerializedObject(switcher);
            }
        }

        public override void OnHeaderGUI()
        {
            if (switcher != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    setIndex = EditorGUILayout.Popup(setIndex, switcher.allKeys.ToArray());
                    if (GUILayout.Button("Apply"))
                    {
                        switcher.Apply(switcher.cases[setIndex].name);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (switcher == null || setIndex >= switcher.cases.Count)
            {
                return;
            }
            so.Update();
            var set = switcher.cases[setIndex];
            using (new EditorGUILayout.VerticalScope())
            {
                var deleteIndex = -1;
                for (int i = 0; i < set.data.Count; ++i)
                {
                    var d = set.data[i];
                    if (d != null)
                    {
                        var type = d.target != null ? d.target.GetType() : typeof(Object);
                        var t = EditorGUILayout.ObjectField(d.target, type, true) as Component;
                        if (t != d.target)
                        {
                            Undo.RecordObject(switcher, "target changed");
                            d.target = t;
                            EditorUtility.SetDirty(switcher);
                        }
                        
                        EditorGUI.indentLevel++;
                        switch (d)
                        {
                            case CompData c:
                                var members = c.ListChangedMembers();
                                if (members.Count > 0)
                                {
                                    foreach (var m in members)
                                    {
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            var p = so.FindProperty($"{nameof(switcher.cases)}.Array.data[{setIndex}].data.Array.data[{i}].{m.name}");
                                            EditorGUILayout.PropertyField(p);
                                            if (GUILayout.Button("-", GUILayout.Width(20)))
                                            {
                                                if (members.Count == 1 && deleteIndex < 0)
                                                {
                                                    deleteIndex = i;
                                                } else
                                                {
                                                    for (int s=0; s<switcher.cases.Count; ++s)
                                                    {
                                                        var isSet = so.FindProperty($"{nameof(switcher.cases)}.Array.data[{s}].data.Array.data[{i}].{m.name}{MemberControl.MOD_SUFFIX}");
                                                        if (isSet != null)
                                                        {
                                                            isSet.boolValue = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                } else
                                {
                                    deleteIndex = i;
                                }
                                break;
                            default:
                                Debug.LogWarning("Not implemented " + d.GetType());
                                break;
                        }
                        EditorGUI.indentLevel--;
                    } else
                    {
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            deleteIndex = i;
                        }
                    }
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }

                if (deleteIndex >= 0)
                {
                    for (int i = 0; i < switcher.cases.Count; ++i)
                    {
                        switcher.cases[i].data.RemoveAt(deleteIndex);
                    }
                    EditorUtility.SetDirty(switcher);
                }
            }
            so.ApplyModifiedProperties();
        }

        public override void OnFooterGUI()
        {
        }

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        public override void OnChangeScene(string sceneName)
        {
        }

        public override void OnChangePlayMode(PlayModeStateChange stateChange)
        {
        }

        public override string ToString()
        {
            return "";
        }
    }
}