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
                Unpack();
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
            var selected = sortedSelection;
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

        [MenuItem("GameObject/UI/Switcher/Unpack", true, 102)]
        public static bool IsUnpack()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Switcher>() != null;
        }


        [MenuItem("GameObject/UI/Switcher/Unpack", false, 102)]
        public static void Unpack()
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

                    var switcher = root0.GetComponent<Switcher>();
                    var actions = switcher?.cases.ConvertAll(c => c.action);
                    string name0 = null;
                    if (switcher != null)
                    {
                        if (switcher.cases.Count > 0)
                        {
                            name0 = switcher.cases[0].name;
                        }
                        switcher.Clear();
                    } else
                    {
                        switcher = root0.AddComponent<Switcher>();
                    }

                    for (var i = 0; i < roots.Count; ++i)
                    {
                        var c = new Case
                        {
                            name = i == 0 && name0 != null? name0: roots[i].name,
                            data = diffs[i],
                            action = actions != null && actions.Count > i? actions[i] : null
                        };
                        switcher.cases.Add(c);
                    }
                    EditorUtility.SetDirty(switcher);
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
    }
}