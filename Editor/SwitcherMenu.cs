//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using mulova.switcher.foundation;
using UnityEditor;
using UnityEngine;

namespace mulova.switcher
{
    public static class SwitcherMenu
    {
        public static List<GameObject> sortedSelection
        {
            get
            {
                var roots = new List<GameObject>(Selection.gameObjects);
                roots.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());
                return roots;
            }
        }

        [MenuItem("GameObject/UI/Switcher/Pack Or Unpack", true, 101)]
        public static bool IsPackOrUnpack()
        {
            return Selection.gameObjects.Length > 1 || (Selection.gameObjects.Length == 1 && Selection.activeGameObject.TryGetComponent<Switcher>(out var s));
        }

        [MenuItem("GameObject/UI/Switcher/Pack Or Unpack %&S", false, 101)]
        public static void PackOrUnpack()
        {
            if (Selection.gameObjects.Length > 1)
            {
                Pack(false);
            } else if (Selection.gameObjects.Length == 1 && Selection.activeGameObject.TryGetComponent<Switcher>(out var s))
            {
                var switcher = Selection.activeGameObject.GetComponent<Switcher>();
                if (switcher != null)
                {
                    switcher.Unpack();
                }
                else
                {
                    Debug.Log("No switcher selected");
                }
            }
        }

        [MenuItem("GameObject/UI/Switcher/Pack with root transform", true, 102)]
        public static bool IsPackWithRootTransform()
        {
            return Selection.gameObjects.Length > 1;
        }

        [MenuItem("GameObject/UI/Switcher/Pack with root transform", false, 102)]
        public static void PackWithRootTransform()
        {
            Pack(true);
        }

        private static int runFrame;
        public static void Pack(bool extractRootDiff)
        {
            var selected = FilterTopLevelSelection(sortedSelection);
            if (selected == null || selected.Count == 1)
            {
                return;
            }
            if (runFrame == Time.frameCount)
            {
                return;
            }
            runFrame = Time.frameCount;

            var rootData = new List<RootData>();
            if (Pack(selected, extractRootDiff))
            {
                for (int i = 1; i < selected.Count; ++i)
                {
                    rootData.Add(new RootData(selected[i].transform));
                }
                for (int i = 1; i < selected.Count; ++i)
                {
                    Undo.DestroyObjectImmediate(selected[i]);
                }
                if (!SwitcherEditorConfig.instance.deleteCases)
                {
                    selected[0].GetComponent<Switcher>().Unpack(rootData, false);
                }
                Selection.activeGameObject = selected[0];
            } else
            {
                EditorUtility.DisplayDialog("Error", "Check out logs for more detail", "OK");
            }
        }

        [MenuItem("GameObject/UI/Switcher/Merge Switchers", true, 103)]
        public static bool IsMergeSwitchers()
        {
            var sel = Selection.gameObjects;
            var doesAllSelectionHaveSwitcher = sel.FindAll(s => s.GetComponent<Switcher>()).Count == sel.Length;
            return sel.Length > 1 && doesAllSelectionHaveSwitcher && DiffExtractor.IsChildrenMatches(sel.ConvertAll(o=>o.transform));
        }

        [MenuItem("GameObject/UI/Switcher/Merge Switchers", false, 103)]
        public static void MergeSwitchers()
        {
            var selected = sortedSelection;
            var switchers = selected.ConvertAll(o => o.GetComponent<Switcher>());
            
            Undo.RecordObjects(switchers.ToArray(), "Merge");
            for (int i=1; i<switchers.Count; ++i)
            {
                switchers[0].Merge(switchers[i]);
            }
            EditorUtil.SetDirty(switchers[0]);
            Selection.objects = new Object[] { selected[0] };
        }

        private static bool Pack(List<GameObject> roots, bool extractRootDiff)
        {
            var duplicates = DiffExtractor.GetDuplicateSiblingNames(roots);
            if (duplicates.Count > 0)
            {
                Debug.LogError("Duplicate sibling names are not allowed: " + string.Join(",", duplicates));
                return false;
            }
            else
            {
                Undo.SetCurrentGroupName("Switcher.Pack");
                var undoGroup = Undo.GetCurrentGroup();
                roots.ForEach(r=> Undo.RegisterFullObjectHierarchyUndo(r, r.name));
                var rootTrans = roots.ConvertAll(o => o.transform);
                DiffExtractor.CreateMissingChildren(rootTrans, rootTrans);
                DiffExtractor.CreateMissingComponent(rootTrans, rootTrans);
                if (DiffExtractor.IsComponentMatch(rootTrans))
                {
                    MakeRootsActive(roots);
                    for (var i=1; i < rootTrans.Count; ++i)
                    {
                        DiffExtractor.ReplaceRefs(rootTrans[i], rootTrans[0], rootTrans[i], rootTrans[0]);
                    }
                    
                    // just set data for the first object
                    var root0 = roots[0];
                    var diffs = DiffExtractor.CreateDiff(roots.ToArray(), extractRootDiff);

                    var switcher0 = root0.GetComponent<Switcher>();
                    if (switcher0 == null)
                    {
                        switcher0 = root0.AddComponent<Switcher>();
                    }

                    for (var i = 0; i < roots.Count; ++i)
                    {
                        var caseName = roots[i].name;
                        SwitcherEvent action = null;
                        var sourceSwitcher = roots[i].GetComponent<Switcher>();
                        if (sourceSwitcher != null)
                        {
                            if (sourceSwitcher.cases.Count > i)
                            {
                                var casei = sourceSwitcher.cases[i];
                                action = casei.action;
                            }
                            if (action != null && roots[i].transform != root0.transform)
                            {
                                action.ReplaceMatchingTarget(roots[i].transform, root0.transform);
                            }
                        }

                        var c = new Case
                        {
                            name = caseName,
                            data = diffs[i],
                            action = action
                        };
                        if (i == 0)
                        {
                            switcher0.Clear();
                        }
                        switcher0.cases.Add(c);
                    }
                    EditorUtility.SetDirty(switcher0);
                    Undo.CollapseUndoOperations(undoGroup);
                    //diffList.serializedProperty.ClearArray();
                    return true;
                } else
                {
                    roots.ForEach(Undo.ClearUndo);
                    return false;
                }
            }

            static void MakeRootsActive(IEnumerable<GameObject> roots)
            {
                foreach (var r in roots)
                {
                    if (!r.activeSelf)
                    {
                        r.SetActive(true);
                    }
                }
            }
        }

        private static List<GameObject> FilterTopLevelSelection(IReadOnlyList<GameObject> selected)
        {
            if (selected == null || selected.Count == 0)
            {
                return new List<GameObject>();
            }
            var unique = new List<GameObject>(selected.Count);
            var set = new HashSet<GameObject>();
            foreach (var go in selected)
            {
                if (go != null && set.Add(go))
                {
                    unique.Add(go);
                }
            }
            var filtered = new List<GameObject>(unique.Count);
            foreach (var go in unique)
            {
                var t = go.transform.parent;
                var isChildOfSelection = false;
                while (t != null)
                {
                    if (set.Contains(t.gameObject))
                    {
                        isChildOfSelection = true;
                        break;
                    }
                    t = t.parent;
                }
                if (!isChildOfSelection)
                {
                    filtered.Add(go);
                }
            }
            filtered.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());
            return filtered;
        }
    }
}
