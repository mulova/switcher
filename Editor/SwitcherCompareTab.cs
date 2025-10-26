//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using mulova.switcher.foundation;

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
        private int caseIndex;
        private SwitcherCompareWindow win => base.window as SwitcherCompareWindow;

        public SwitcherCompareTab(int index, TabbedEditorWindow window) : base(window)
        {
            this.caseIndex = index;
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
                    caseIndex = EditorGUILayout.Popup(caseIndex, switcher.allKeys.ToArray());
                    if (GUILayout.Button("Apply"))
                    {
                        switcher.Apply(switcher.cases[caseIndex].name);
                    }
                }
            }
        }

        private List<int> addIndex = new ();
        public override void OnInspectorGUI()
        {
            if (switcher == null || caseIndex >= switcher.cases.Count)
            {
                return;
            }
            var so = win.serializedObject;
            so.Update();
            var set = switcher.cases[caseIndex];
            using (new EditorGUILayout.VerticalScope())
            {
                var deleteIndex = -1;
                for (int dataIndex = 0; dataIndex < set.data.Count; ++dataIndex)
                {
                    if (win.hideData.Contains(dataIndex))
                    {
                        continue;
                    }
                    var d = set.data[dataIndex];
                    if (d != null)
                    {
                        var type = d.srcType;
                        using (new HorizontalScope())
                        {
                            using (new EnableScope(caseIndex == 0))
                            {
                                var t = EditorGUILayout.ObjectField(d.target, type, true) as Component;
                                if (t != d.target)
                                {
                                    Undo.RecordObject(switcher, "target changed");
                                    foreach (var c in switcher.cases)
                                    {
                                        c.data[dataIndex].target = t;
                                    }
                                    EditorUtility.SetDirty(switcher);
                                }
                            }
                            if (caseIndex == 0 && GUILayout.Button("Hide", GUILayout.ExpandWidth(false)))
                            {
                                win.hideData.Add(dataIndex);
                            }
                        }
                        
                        EditorGUI.indentLevel++;
                        var members = switcher.ListChangedMembers(dataIndex);
                        if (members.Count > 0)
                        {
                            foreach (var m in members)
                            {
                                using (new EnableScope(m.IsChanged(d)))
                                // using (new EnableScope(switcher.IsChangeable(set, c, m)))
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        var p = FindMemberProperty(so, caseIndex, dataIndex, m.name);
                                        EditorGUILayout.PropertyField(p);
                                        using (new EnableScope(GUI.enabled || (caseIndex == 0 && d.target == null)))
                                        {
                                            if (GUILayout.Button("-", GUILayout.Width(20)))
                                            {
                                                if (members.Count == 1 && deleteIndex < 0)
                                                {
                                                    deleteIndex = dataIndex;
                                                }
                                                else
                                                {
                                                    for (int s = 0; s < switcher.cases.Count; ++s)
                                                    {
                                                        var isSet = FindMemberProperty(so, s, dataIndex, m.name+MemberControl.MOD_SUFFIX);
                                                        if (isSet != null)
                                                        {
                                                            isSet.boolValue = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        } else
                        {
                            deleteIndex = dataIndex;
                        }
                        if (win.isAdd)
                        {
                            var unchangedMembers = new List<MemberControl>(d.ListUnchangedMembers());
                            if (unchangedMembers.Count > 0)
                            {
                                unchangedMembers.Sort((a,b)=> a.name.CompareTo(b.name));
                                var names = unchangedMembers.ConvertAll(m => m.name).ToArray();
                                while (addIndex.Count <= dataIndex)
                                {
                                    addIndex.Add(0);
                                }
                                addIndex[dataIndex] = Mathf.Clamp(addIndex[dataIndex], 0, names.Length - 1);
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    addIndex[dataIndex] = EditorGUILayout.Popup(addIndex[dataIndex], names);
                                    if (GUILayout.Button("+", GUILayout.Width(20)))
                                    {
                                        var m = unchangedMembers[addIndex[dataIndex]];
                                        var p0 = FindMemberProperty(so, 0, dataIndex, m.name);
                                        var val = p0.GetValue();
                                        Undo.RecordObject(switcher, "Add " + m.ToString());
                                        var isSet = FindMemberProperty(so, caseIndex, dataIndex, m.name + MemberControl.MOD_SUFFIX);
                                        if (isSet != null)
                                        {
                                            isSet.boolValue = true;
                                        }
                                        var p = FindMemberProperty(so, caseIndex, dataIndex, m.name);
                                        p.SetValue(val);
                                    }
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    } else
                    {
                        // TODO: use default property drawer
                        EditorGUILayout.PropertyField(FindDataProperty(so, caseIndex, dataIndex), new GUIContent(""), true);
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            deleteIndex = dataIndex;
                        }
                    }
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }

                if (deleteIndex >= 0)
                {
                    if (switcher.cases[0].data[deleteIndex] != null)
                    {
                        Undo.RecordObject(switcher, "Remove "+ switcher.cases[0].data[deleteIndex].ToString());
                    }
                    for (int i = 0; i < switcher.cases.Count; ++i)
                    {
                        switcher.cases[i].data.RemoveAt(deleteIndex);
                    }
                    EditorUtility.SetDirty(switcher);
                }
            }
            so.ApplyModifiedProperties();
        }

        public static string GetDataPropertyPath(int caseIndex, int dataIndex) => $"{nameof(Switcher.cases)}.Array.data[{caseIndex}].data.Array.data[{dataIndex}]";
        public static string GetMemberPropertyPath(int caseIndex, int dataIndex, string memberName) => $"{GetDataPropertyPath(caseIndex, dataIndex)}.{memberName}";
        private SerializedProperty FindDataProperty(SerializedObject so, int caseIndex, int dataIndex) => so.FindProperty(GetDataPropertyPath(caseIndex, dataIndex));
        private SerializedProperty FindMemberProperty(SerializedObject so, int caseIndex, int dataIndex, string memberName) => so.FindProperty(GetMemberPropertyPath(caseIndex, dataIndex, memberName));

        public override void OnFooterGUI()
        {
            var dragged = GetDraggedComponent();
            if (dragged != null && switcher.cases[0].GetCompData(dragged) == null)
            {
                var so = win.serializedObject;
                so.Update();
                for (var i = 0; i < switcher.cases.Count; ++i)
                {
                    var inst = CompDataFactory.instance.GetComponentData(dragged);
                    if (inst != null)
                    {
                        var m = inst.GetMember("enabled");
                        m.SetChanged(inst, true);
                        var c = switcher.cases[i];
                        c.data.Add(inst);
                    }
                }
                so.ApplyModifiedProperties();
            }
            
            Component GetDraggedComponent()
            {
                var evt = Event.current;      //현재 이벤트 얻어오기.
                switch (evt.type)
                {
                    case EventType.DragExited:
                    case EventType.DragPerform:
                    {
                        if (win != EditorWindow.mouseOverWindow)
                        {
                            break;
                        }
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();   // 드래그앤 드랍을 허용함.
                        foreach (var draggedObj in DragAndDrop.objectReferences)    // objectReferences: 드래그한 오브젝트들의 레퍼런스
                        {
                            var c = draggedObj as Component;
                            if (c)
                            {
                                Event.current.Use();
                                return c;
                            }
                        }
                    }
                    break;
                }
                return null;
            }
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