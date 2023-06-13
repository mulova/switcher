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
    using UnityEngine.Assertions;
    using Object = UnityEngine.Object;

    public static class DiffExtractor
    {
        public static void ReplaceRefs(Transform trans, Transform rc, Transform r0)
        {
            //Assert.IsTrue(trans == rc || trans.IsChildOf(rc));
            var comps = trans.GetComponents<Component>();
            foreach (var c in comps)
            {
                var data = CompDataFactory.instance.GetComponentData(c);
                if (data != null)
                {
                    var members = data.ListAttributedMembers();
                    foreach (var m in members)
                    {
                        if (!m.attr.manual)
                        {
                            m.ReplaceRefs(c, rc, r0);
                        }
                    }
                }
            }
            // Recursive call
            foreach (Transform child in trans)
            {
                ReplaceRefs(child, rc, r0);
            }
        }

        public static void CreateMissingChildren(IList<Transform> roots)
        {
            var union = GetChildUnion(roots);

            foreach (var p in roots)
            {
                int insertIndex = 0;
                for (int ic = 0; ic < union.Count; ++ic)
                {
                    var c = union[ic];
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
                        var clone = CloneSibling(c.child, p, insertIndex);
                        ReplaceRefs(clone, c.root, p);
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
                arr[i] = CompDataFactory.instance.GetComponentData(comps[i][index]);
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
                            changed = !((Component)v0).transform.IsHierarchyPair(((Component)vi).transform);
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

        public static Transform CloneSibling(Transform src, Transform parent, int siblingIndex)
        {
            if (src == null)
            {
                return null;
            }
            Transform clone;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(src.gameObject))
            {
                var p = PrefabUtility.GetCorrespondingObjectFromOriginalSource(src.gameObject);
                clone = (PrefabUtility.InstantiatePrefab(p, parent) as GameObject).transform;
            } else
            {
                clone = Object.Instantiate(src, parent, false);
            }
            clone.name = src.name;
            Undo.RegisterCreatedObjectUndo(clone.gameObject, src.name);
            clone.SetSiblingIndex(siblingIndex);
            return clone;
        }

        class RootNChild
        {
            public readonly Transform root;
            public readonly Transform child;

            public string name => child.name;

            public RootNChild(Transform root, Transform child)
            {
                this.root = root;
                this.child = child;
            }

        }
        private static List<RootNChild> GetChildUnion(IList<Transform> parents)
        {
            List<Transform> sorted = new List<Transform>(parents);
            sorted.Sort(SortByChildCount);

            int SortByChildCount(Transform t1, Transform t2)
            {
                return t2.childCount - t1.childCount;
            }

            List<RootNChild> union = new List<RootNChild>();
            for (int i=0; i < parents[0].childCount; ++i)
            {
                union.Add(new RootNChild (parents[0], parents[0].GetChild(i)));
            }

            for (int i=1; i<parents.Count; ++i)
            {
                var p = parents[i];
                var oldIndex = -1;
                for (int j = 0; j < p.childCount; ++j)
                {
                    var c = p.GetChild(j);
                    int index = union.FindIndex(t => t.name == c.name);
                    if (index < 0)
                    {
                        for (var k = j + 1; k < p.childCount && index < 0; ++k)
                        {
                            index = union.FindIndex(t => t.name == p.GetChild(k).name);
                        }
                        if (index < 0)
                        {
                            index = union.Count;
                            union.Add(new RootNChild(p, c));
                        }
                        else
                        {
                            union.Insert(index, new RootNChild(p, c));
                        }
                    }
                    if (oldIndex > index)
                    {
                        throw new System.Exception($"Sibling order mismatch: {c.GetScenePath()}");
                    }
                    oldIndex = index;
                }
            }
            return union;
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