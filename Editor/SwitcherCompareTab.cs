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
    using static UnityEngine.GUILayout;

    public class SwitcherCompareTab : EditorTab
    {
        //private WindowToolbar toolbar = new WindowToolbar();
        internal Switcher switcher => targetObject as Switcher;
        private int setIndex;
        private SwitcherCompareWindow win => base.window as SwitcherCompareWindow;

        public SwitcherCompareTab(int index, TabbedEditorWindow window) : base(window)
        {
            this.setIndex = index;
        }

        public override void AddContextMenu() {}
        public override void OnTabChange(bool sel) { }
        public override void OnFocus(bool focus) { }

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

        private List<int> addIndex = new ();
        public override void OnInspectorGUI()
        {
            if (switcher == null || setIndex >= switcher.cases.Count)
            {
                return;
            }
            var so = win.serializedObject;
            so.Update();
            var set = switcher.cases[setIndex];
            using (new EditorGUILayout.VerticalScope())
            {
                var deleteIndex = -1;
                for (int i = 0; i < set.data.Count; ++i)
                {
                    if (win.hideData.Contains(i))
                    {
                        continue;
                    }
                    var d = set.data[i];
                    if (d != null)
                    {
                        var type = d.target != null ? d.target.GetType() : typeof(Object);
                        using (new HorizontalScope())
                        {
                            var t = EditorGUILayout.ObjectField(d.target, type, true) as Component;
                            if (t != d.target)
                            {
                                Undo.RecordObject(switcher, "target changed");
                                d.target = t;
                                EditorUtility.SetDirty(switcher);
                            }
                            if (setIndex == 0 && GUILayout.Button("Hide", GUILayout.ExpandWidth(false)))
                            {
                                win.hideData.Add(i);
                            }
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
                                            var p = FindProperty(so, setIndex, i, m.name);
                                            EditorGUILayout.PropertyField(p);
                                            if (GUILayout.Button("-", GUILayout.Width(20)))
                                            {
                                                if (members.Count == 1 && deleteIndex < 0)
                                                {
                                                    deleteIndex = i;
                                                }
                                                else
                                                {
                                                    for (int s = 0; s < switcher.cases.Count; ++s)
                                                    {
                                                        var isSet = FindProperty(so, s, i, m.name+MemberControl.MOD_SUFFIX);
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
                                if (setIndex == 0 && win.isAdd)
                                {
                                    members = c.ListUnchangedMembers();
                                    if (members.Count > 0)
                                    {
                                        members.Sort((a,b)=> a.name.CompareTo(b.name));
                                        var names = members.ConvertAll(m => m.name).ToArray();
                                        while (addIndex.Count <= i)
                                        {
                                            addIndex.Add(0);
                                        }
                                        addIndex[i] = Mathf.Clamp(addIndex[i], 0, names.Length - 1);
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            addIndex[i] = EditorGUILayout.Popup(addIndex[i], names);
                                            if (GUILayout.Button("+", GUILayout.Width(20)))
                                            {
                                                var m = members[addIndex[i]];
                                                var p0 = FindProperty(so, 0, i, m.name);
                                                var val = p0.GetValue();
                                                Undo.RecordObject(switcher, "Add " + m.ToString());
                                                for (int s = 0; s < switcher.cases.Count; ++s)
                                                {
                                                    var isSet = FindProperty(so, s, i, m.name + MemberControl.MOD_SUFFIX);
                                                    if (isSet != null)
                                                    {
                                                        isSet.boolValue = true;
                                                    }
                                                    var p = FindProperty(so, s, i, m.name);
                                                    p.SetValue(val);
                                                }
                                            }
                                        }
                                    }
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
                    Undo.RecordObject(switcher, "Remove "+ switcher.cases[0].data[deleteIndex].ToString());
                    for (int i = 0; i < switcher.cases.Count; ++i)
                    {
                        switcher.cases[i].data.RemoveAt(deleteIndex);
                    }
                    EditorUtility.SetDirty(switcher);
                }
            }
            so.ApplyModifiedProperties();
        }

        private SerializedProperty FindProperty(SerializedObject so, int setIndex, int dataIndex, string memberName) => so.FindProperty($"{nameof(switcher.cases)}.Array.data[{setIndex}].data.Array.data[{dataIndex}].{memberName}");

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