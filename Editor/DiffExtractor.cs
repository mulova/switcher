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
    using Object = UnityEngine.Object;

    public static class DiffExtractor
    {
        public static void CreateMissingChildren(IList<Transform> roots)
        {
            var children = GetChildUnion(roots);

            foreach (var p in roots)
            {
                int insertIndex = 0;
                for (int ic = 0; ic < children.Count; ++ic)
                {
                    var c = children[ic];
                    // find matching child
                    Transform found = null;
                    for (int i = 0; i < p.childCount && !found; ++i)
                    {
                        if (p.GetChild(i).name == c.name)
                        {
                            insertIndex = i + 1;
                            found = p.GetChild(i);
                        }
                    }
                    if (found == null)
                    {
                        var clone = CloneSibling(c, p, insertIndex);
                        clone.gameObject.SetActive(false);
                        ++insertIndex;
                    }
                }
            }

            // sort children
            for (int i=0; i<roots[0].childCount; ++i)
            {
                var c = roots[0].GetChild(i);

                var childRoots = new List<Transform>();
                for (int j=0; j < roots.Count; ++j)
                {
                    childRoots.Add(roots[j].GetChild(i));
                    if (j != 0)
                    {
                        roots[j].Find(c.name).SetSiblingIndex(i);
                    }
                }
                CreateMissingChildren(childRoots);
            }
        }

        public static List<ICompData>[] CreateDiff(GameObject[] roots)
        {
            var trans = roots.ConvertAll(o => o.transform);
            var store = trans.ConvertAll(p => new List<ICompData>());
            GetDiffRecursively(trans, trans, store, 0);
            return store;
        }

        private static void GetDiffRecursively(Transform[] roots, Transform[] current, List<ICompData>[] store, int depth)
        {
            if (roots[0].TryGetComponent<IgnoreSwitch>(out _) || (depth > 0 && current[0].TryGetComponent<Switcher>(out _)))
            {
                return;
            }
            var comps = current.ConvertAll(p => p.GetComponents<Component>().FindAll(c=> !(c is Switcher)).ToArray());
            for (int i = 0; i < comps[0].Length; ++i)
            {
                GetMatchingComponentDiff(roots, comps, i, store);
            }
            // child diff
            for (int i=0; i < current[0].childCount; ++i)
            {
                var children = current.ConvertAll(p => p.GetChild(i));
                GetDiffRecursively(roots, children, store, depth+1);
            }
        }

        /// <summary>
        /// return Component data if all components' data are the same.
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="comps">return Component data if all components' data are the same.</param>
        private static void GetMatchingComponentDiff(Transform[] roots, Component[][] comps, int index, List<ICompData>[] store)
        {
            var arr = new ICompData[comps.Length];
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = CompDataGenerator.instance.GetComponentData(comps[i][index], roots[i], roots[0]);
            }
            if (arr[0] != null)
            {
                var diff = GetDiffs(arr);
                if (diff)
                {
                    for (int i = 0; i < arr.Length; ++i)
                    {
                        arr[i].target = arr[0].target;
                        store[i].Add(arr[i]);
                    }
                }
            }
        }

        public static bool GetDiffs(ICompData[] data)
        {
            var members = MemberControl.ListAttributedMembers(data[0].srcType, data[0].GetType(), null);
            var anyChanged = false;
            var isTransform = typeof(Transform).IsAssignableFrom(data[0].srcType);
            var rectChanged = false;
            foreach (var m in members)
            {
                var changed = false;
                var v0 = m.GetValue(data[0]);
                for (int i = 1; i < data.Length && !changed; ++i)
                {
                    var active = data[i].target.gameObject.activeInHierarchy;
                    if (!isTransform && !active) // ignore compoennt value except TransformData.enabled (GameObject.active) values
                    {
                        continue;
                    }
                    var vi = m.GetValue(data[i]);
                    if (isTransform && !active && m.name != "enabled") // ignore inactive transform values except GameObject.active
                    {
                        continue;
                    }
                    if (v0 == null ^ vi == null)
                    {
                        changed = true;
                    } else if (v0 is Object o0 && o0 != (System.Object)vi)
                    {
                        changed = true;
                    }
                    else if (v0 != null && !data[0].MemberEquals(m, v0, vi))
                    {
                        if (m.isReference && v0.GetType() == vi.GetType())
                        {
                            changed = !MemberControl.IsHierarchyMatch(((Component)v0).transform, ((Component)vi).transform);
                        }
                        else
                        {
                            changed = true;
                        }
                    }
                }
                if (typeof(RectTransform).IsAssignableFrom(data[0].srcType) && m.name != "enabled" && changed)
                {
                    rectChanged = true;
                }
                anyChanged |= changed;
                foreach (var d in data)
                {
                    m.SetChanged(d, changed);
                }
            }
            if (rectChanged)
            {
                foreach (var s in data)
                {
                    members.Find(m => m.name == "anchoredPosition").SetChanged(s, true);
                }
            }
            return anyChanged;
        }

        public static List<string> GetDuplicateSiblingNames(IList<GameObject> objs)
        {
            List<string> dup = new List<string>();
            foreach (var o in objs)
            {
                if (o != null)
                {
                    dup.AddRange(GetDuplicateSiblingNames(o.transform));
                }
            }
            return dup;

            List<string> GetDuplicateSiblingNames(Transform parent)
            {
                List<string> d = new List<string>();
                HashSet<string> names = new HashSet<string>();
                foreach (Transform child in parent)
                {
                    if (!names.Add(child.name))
                    {
                        d.Add(child.name);
                    }
                    d.AddRange(GetDuplicateSiblingNames(child));
                }
                return d;
            }
        }

        public static bool IsChildrenMatches(IList<Transform> objs)
        {
            int childCount = objs[0].childCount;
            for (int i = 1; i < objs.Count; ++i)
            {
                if (objs[i].childCount != childCount)
                {
                    return false;
                }
            }
            Transform[] children = new Transform[objs.Count];
            for (int c=0; c<childCount; ++c)
            {
                for (int i=0; i<objs.Count; ++i)
                {
                    children[i] = objs[i].GetChild(c);
                    if (i != 0 && children[0].name != children[i].name)
                    {
                        return false;
                    }
                }
                if (!IsChildrenMatches(children))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsComponentMatch(IList<Transform> objs)
        {
            var passed = true;
            var comps = new List<Component>[objs.Count];
            var count = -1;
            for (int i = 0; i < objs.Count; ++i)
            {
                comps[i] = objs[i].GetComponents<Component>().FindAll(co => !(co is Switcher));
                if (i == 0)
                {
                    count = comps[i].Count;
                } else if (count != comps[i].Count)
                {
                    passed = false;
                }
            }
            if (passed)
            {
                for (int i = 0; i < comps[0].Count; ++i)
                {
                    for (int j = 1; j < comps.Length; ++j)
                    {
                        if (comps[0][i].GetType() != comps[j][i].GetType())
                        {
                            for (int k = 0; k < comps.Length; ++k)
                            {
                                Debug.LogError($"Type Mismatch({k}/{comps[0].Count}): {comps[k][i].name} {comps[k][i].GetType().Name}({i})", comps[k][i]);
                            }
                            passed = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < objs.Count; ++i)
                {
                    Debug.LogError($"Component count mismatch({i+1}/{objs.Count}): '{objs[i].transform.GetScenePath()}' ({comps[i].Count})", objs[i]);
                }
            }
            for (int c=0; c<objs[0].childCount; ++c)
            {
                List<Transform> children = new List<Transform>();
                for (int i=0; i<objs.Count; ++i)
                {
                    children.Add(objs[i].GetChild(c));
                }
                passed &= IsComponentMatch(children);
            }
            return passed;
        }

        public static int GetSiblingIndex(string objName, Transform parent)
        {
            for (int i = 0; i < parent.childCount; ++i)
            {
                var c = parent.GetChild(i);
                if (c.name == objName)
                {
                    return i;
                }
            }
            return -1;
        }

        public static Transform CloneSibling(Transform c1, Transform parent, int siblingIndex)
        {
            if (c1 == null)
            {
                return null;
            }
            Transform child = null;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(c1.gameObject))
            {
                var p = PrefabUtility.GetCorrespondingObjectFromOriginalSource(c1.gameObject);
                child = (PrefabUtility.InstantiatePrefab(p, parent) as GameObject).transform;
            } else
            {
                child = Object.Instantiate(c1, parent, false);
            }
            child.name = c1.name;
            Undo.RegisterCreatedObjectUndo(child.gameObject, c1.name);
            child.SetSiblingIndex(siblingIndex);
            return child;
        }

        private static List<Transform> GetChildUnion(IList<Transform> parents)
        {
            List<Transform> sorted = new List<Transform>(parents);
            sorted.Sort(SortByChildCount);

            int SortByChildCount(Transform t1, Transform t2)
            {
                return t2.childCount - t1.childCount;
            }

            List<Transform> names = new List<Transform>();
            foreach (var p in parents)
            {
                for (int i = 0; i < p.childCount; ++i)
                {
                    var c = p.GetChild(i);
                    int index = names.FindIndex(t => t.name == c.name);
                    if (index < 0)
                    {
                        for (var j = i + 1; j < p.childCount && index < 0; ++j)
                        {
                            index = names.FindIndex(t => t.name == p.GetChild(j).name);
                        }
                        if (index < 0)
                        {
                            names.Add(c);
                        }
                        else
                        {
                            names.Insert(index, c);
                        }
                    }
                }
            }
            return names;
        }

        internal static List<T>[] FindAll<T>(List<ICompData>[] diffs) where T : ICompData
        {
            var list = new List<T>[diffs.Length];
            for (int i=0; i<diffs.Length; ++i)
            {
                list[i] = diffs[i].FindAll(c => c is T).ConvertAll(t => (T)t);
            }
            return list;
        }
    }
}