//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
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

        [MenuItem("GameObject/UI/Switcher/Generate", true, 101)]
        public static bool IsCreateSwitcher()
        {
            return Selection.gameObjects.Length > 1;
        }

        private static int runFrame;
        [MenuItem("GameObject/UI/Switcher/Generate", false, 101)]
        public static void CreateSwitcher()
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
            if (CreateSwitcher(selected))
            {
                for (int i = 1; i < selected.Count; ++i)
                {
                    rootData.Add(new RootData(selected[i].transform));
                }
                for (int i = 1; i < selected.Count; ++i)
                {
                    Undo.DestroyObjectImmediate(selected[i]);
                }
                selected[0].GetComponent<Switcher>().SpreadOut(rootData, false);
                Selection.activeGameObject = selected[0];
            } else
            {
                EditorUtility.DisplayDialog("Error", "Check out logs for more detail", "OK");
            }
        }

        [MenuItem("GameObject/UI/Switcher/Spread Out", true, 102)]
        public static bool IsSpreadOut()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Switcher>();
        }


        [MenuItem("GameObject/UI/Switcher/Spread Out", false, 102)]
        public static void SpreadOut()
        {
            Selection.activeGameObject.GetComponent<Switcher>().SpreadOut();
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
            Selection.objects = new[] { selected[0] };
        }

        public static bool CreateSwitcher(List<GameObject> roots)
        {
            var duplicates = DiffExtractor.GetDuplicateSiblingNames(roots);
            if (duplicates.Count > 0)
            {
                Debug.LogError("Duplicate sibling names: " + string.Join(",", duplicates));
                return false;
            }
            else
            {
                if (!DiffExtractor.IsChildrenMatches(roots.ConvertAll(o => o.transform)))
                {
                    var parents = roots.ConvertAll(o => o.transform);
                    DiffExtractor.CreateMissingChildren(parents);
                }
                if (DiffExtractor.IsComponentMatch(roots.ConvertAll(o => o.transform)))
                {
                    Undo.RecordObjects(roots.ToArray(), "Diff");
                    ExtractDiff(roots);
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        private static void ExtractDiff(List<GameObject> roots)
        {
            // just set data for the first object
            var root0 = roots[0];
            var diffs = DiffExtractor.CreateDiff(roots.ToArray());
            var tDiffs = DiffExtractor.FindAll<TransformData>(diffs);
            // remove TransformData from diffs
            //for (int i = 0; i < diffs.Length; ++i)
            //{
            //    diffs[i] = diffs[i].FindAll(d => !(d.GetType() == typeof(TransformData)));
            //}

            var switcher = root0.GetComponent<Switcher>();
            string firstId = null;
            if (switcher != null)
            {
                if (switcher.switches.Count > 0)
                {
                    firstId = switcher.switches[0].name;
                }
                switcher.Clear();
            } else
            {
                switcher = root0.AddComponent<Switcher>();
                Undo.RegisterCreatedObjectUndo(switcher, root0.name);
            }
            // Get Visibility Diffs
            var showDiffs = new List<List<TransformData>>();
            for (int i = 0; i < tDiffs.Length; ++i)
            {
                showDiffs.Add(new List<TransformData>());
            }
            for (int c = 0; c < tDiffs[0].Count; ++c)
            {
                bool diff = false;
                for (int i = 1; i < tDiffs.Length && !diff; ++i)
                {
                    diff |= tDiffs[0][c].active != tDiffs[i][c].active;
                }
                if (diff)
                {
                    for (int i = 0; i < tDiffs.Length; ++i)
                    {
                        showDiffs[i].Add(tDiffs[i][c]);
                    }
                }
            }

            // Get Position Diffs
            var posDiffs = new List<List<TransformData>>();
            for (int i = 0; i < tDiffs.Length; ++i)
            {
                posDiffs.Add(new List<TransformData>());
            }
            for (int c = 0; c < tDiffs[0].Count; ++c)
            {
                bool diff = false;
                for (int i = 1; i < tDiffs.Length && !diff; ++i)
                {
                    diff |= !tDiffs[0][c].TransformEquals(tDiffs[i][c]);
                }
                if (diff)
                {
                    for (int i = 0; i < tDiffs.Length; ++i)
                    {
                        posDiffs[i].Add(tDiffs[i][c]);
                    }
                }
            }

            for (int i = 0; i < roots.Count; ++i)
            {
                var s = new SwitchSet();
                s.name = i == 0 && firstId != null? firstId: roots[i].name;
#if UNITY_2019_1_OR_NEWER
                s.data = diffs[i];
#endif
                switcher.switches.Add(s);
            }
            EditorUtility.SetDirty(switcher);
            //diffList.serializedProperty.ClearArray();
        }
    }
}