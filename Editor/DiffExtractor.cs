//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Object = UnityEngine.Object;

    internal static class DiffExtractor
    {
        internal static void ReplaceRefs(Transform trans, Transform trans0, Transform root, Transform root0)
        {
            var comps = trans.GetComponents<Component>();
            var comps0 = trans0.GetComponents<Component>();
            for (var i=0; i<comps.Length; ++i)
            {
                ReplaceRefs(comps[i], comps0[i], root, root0);
            }
            // Recursive call
            for (var i=0; i<trans.GetSerializedChildCount(); ++i)
            {
                ReplaceRefs(trans.GetSerializedChild(i), trans0.GetSerializedChild(i), root, root0);
            }
        }

        private static void ReplaceRefs(Component c, Component c0, Transform root, Transform root0)
        {
            var data = CompDataFactory.instance.GetComponentData(c);
            if (data != null)
            {
                data.ReplaceRefs(c, c0, root, root0);
            }
        }

        internal static void CreateMissingChildren(IReadOnlyList<Transform> parents, IReadOnlyList<Transform> roots)
        {
            var union = GetChildUnion(parents, roots);

            for (int ip=0; ip<parents.Count; ++ip)
            {
                var p = parents[ip];
                int insertIndex = 0;
                for (int ic = 0; ic < union.Count; ++ic)
                {
                    var c = union[ic];
                    // find matching child
                    Transform found = null;
                    for (int i = 0; i < p.GetSerializedChildCount() && !found; ++i)
                    {
                        if (p.GetSerializedChild(i).name == c.name)
                        {
                            insertIndex = i + 1;
                            found = p.GetSerializedChild(i);
                        }
                    }
                    if (found == null)
                    {
                        var clone = CloneSibling(c.comp, p, insertIndex);
                        ReplaceRefs(clone, c.comp, c.root, roots[ip]);
                        clone.gameObject.SetActive(false);
                        ++insertIndex;
                    }
                }
            }

            // sort children
            for (int i=0; i< parents[0].GetSerializedChildCount(); ++i)
            {
                var c = parents[0].GetSerializedChild(i);

                var childRoots = new List<Transform>();
                for (int j=0; j < parents.Count; ++j)
                {
                    childRoots.Add(parents[j].GetSerializedChild(i));
                    if (j != 0)
                    {
                        parents[j].Find(c.name).SetSiblingIndex(i);
                    }
                }
                CreateMissingChildren(childRoots, roots);
            }

            static List<RootNTransform> GetChildUnion(IReadOnlyList<Transform> parents, IReadOnlyList<Transform> roots)
            {
                Assert.AreEqual(parents.Count, roots.Count);
                List<RootNTransform> union = new List<RootNTransform>();
                for (int i = 0; i < parents[0].GetSerializedChildCount(); ++i)
                {
                    union.Add(new RootNTransform(roots[0], parents[0].GetSerializedChild(i)));
                }

                for (int ip = 1; ip < parents.Count; ++ip)
                {
                    var parent = parents[ip];
                    var oldIndex = -1;
                    for (int ic = 0; ic < parent.GetSerializedChildCount(); ++ic)
                    {
                        var c = parent.GetSerializedChild(ic);
                        int index = union.FindIndex(t => t.name == c.name);
                        if (index < 0)
                        {
                            // find next component index
                            for (var ic2 = ic + 1; ic2 < parent.GetSerializedChildCount() && index < 0; ++ic2)
                            {
                                index = union.FindIndex(t => t.name == parent.GetSerializedChild(ic2).name);
                            }
                            if (index < 0)
                            {
                                index = union.Count;
                                union.Add(new RootNTransform(roots[ip], c));
                            }
                            else
                            {
                                union.Insert(index, new RootNTransform(roots[ip], c));
                            }
                        }
                        if (oldIndex > index)
                        {
                            var msg = $"Sibling order mismatch: {c.GetScenePath()}";
                            Debug.LogError(msg, c);
                            throw new System.Exception(msg);
                        }
                        oldIndex = index;
                    }
                }
                return union;
            }

            static Transform CloneSibling(Transform src, Transform parent, int siblingIndex)
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
                }
                else
                {
                    clone = Object.Instantiate(src, parent, false);
                }
                clone.name = src.name;
                Undo.RegisterCreatedObjectUndo(clone.gameObject, src.name);
                clone.SetSiblingIndex(siblingIndex);
                return clone;
            }
        }

        internal static void CreateMissingComponent(IReadOnlyList<Transform> objs, IReadOnlyList<Transform> roots)
        {
            var union = GetComponentUnion(objs, roots);

            for (int ip = 0; ip < objs.Count; ++ip)
            {
                var o = objs[ip];
                int insertIndex = 0;
                for (int ic = 0; ic < union.Count; ++ic)
                {
                    var c = union[ic];
                    var comps = o.GetComponents<Behaviour>();
                    // find matching component
                    Component found = null;
                    for (int i = 0; i < comps.Length && !found; ++i)
                    {
                        if (comps[i].GetType() == c.type)
                        {
                            insertIndex = i + 1;
                            found = comps[i];
                        }
                    }
                    if (found == null)
                    {
                        var clone = CloneComponent(c.comp, o, insertIndex);
                        ReplaceRefs(clone, c.comp, c.root, roots[ip]);
                        clone.enabled = false;
                        ++insertIndex;
                    }
                }
            }

            // sort children
            for (int i = 0; i < objs[0].GetSerializedChildCount(); ++i)
            {
                var c = objs[0].GetSerializedChild(i);

                var childRoots = new List<Transform>();
                for (int j = 0; j < objs.Count; ++j)
                {
                    childRoots.Add(objs[j].GetSerializedChild(i));
                    if (j != 0)
                    {
                        objs[j].Find(c.name).SetSiblingIndex(i);
                    }
                }
                CreateMissingComponent(childRoots, roots);
            }

            static List<Behaviour> GetAllComponents(Transform t)
            {
                var all = t.GetComponents<Behaviour>();
                var filtered = SwitcherEditorConfig.instance.recordNestedSwitcherData? all: all.Where(c => c.GetType() != typeof(Switcher));
                return filtered.ToList();
            }

            static List<RootNComp> GetComponentUnion(IReadOnlyList<Transform> objs, IReadOnlyList<Transform> roots)
            {
                Assert.AreEqual(objs.Count, roots.Count);
                var all = GetAllComponents(objs[0]);
                //var union = all.ConvertAll(c => new RootNComp(roots[0], c));
                var u = new List<RootNComp>(all.Count);
                foreach (var a in all)
                {
                    u.Add(new RootNComp(roots[0], a));
                }

                for (int ip = 1; ip < objs.Count; ++ip)
                {
                    var o = objs[ip];
                    var comps = GetAllComponents(o);
                    var oldIndex = -1;
                    for (int ic = 0; ic < comps.Count; ++ic)
                    {
                        var c = comps[ic];
                        int index = u.FindIndex(t => t.type == c.GetType());
                        if (index < 0)
                        {
                            // find next component index
                            for (var ic2 = ic + 1; ic2 < comps.Count && index < 0; ++ic2)
                            {
                                index = u.FindIndex(t => t.type == comps[ic2].GetType());
                            }
                            if (index < 0)
                            {
                                index = u.Count;
                                u.Add(new RootNComp(roots[ip], c));
                            }
                            else
                            {
                                u.Insert(index, new RootNComp(roots[ip], c));
                            }
                        }
                        if (oldIndex > index)
                        {
                            throw new System.Exception($"Component order mismatch: {c.transform.GetScenePath()}[{c.GetType().Name}]");
                        }
                        oldIndex = index;
                    }
                }
                return u;
            }

            static Behaviour CloneComponent(Behaviour c, Transform obj, int compIndex)
            {
                if (c == null)
                {
                    return null;
                }
                if (PrefabUtility.IsPartOfAnyPrefab(c))
                {
                    throw new System.Exception($"Can't add Component in prefab instance: {c.transform.GetScenePath()}[{c.GetType().Name}]");
                }
                else
                {
                    var add = obj.gameObject.AddComponent(c.GetType());
                    if (add == null)
                    {
                        var msg = $"Fail to add {c.GetType().Name} to {obj.transform.GetScenePath()}";
                        Debug.LogError(msg, obj);
                        throw new Exception(msg);
                    }
                    Behaviour clone = add as Behaviour;
                    Undo.RegisterCreatedObjectUndo(clone.gameObject, c.GetType().Name);
                    // change order
                    var len = obj.GetComponents<Behaviour>().Length;
                    for (int i=len-1; i>compIndex; --i)
                    {
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(clone);
                    }
                    return clone;
                }
            }
        }

        internal static List<CompData>[] CreateDiff(GameObject[] roots, bool extractRootDiff)
        {
            var trans = roots.ConvertAll(o => o.transform);
            var store = trans.ConvertAll(p => new List<CompData>());
            GetDiffRecursively(trans, trans, store, 0, extractRootDiff);
            return store;
        }

        private static void GetDiffRecursively(Transform[] roots, Transform[] current, List<CompData>[] store, int depth, bool extractRootDiff)
        {
            if (roots[0].TryGetComponent<IgnoreSwitch>(out _))
            {
                return;
            }
            var bypassNestedSwitcher = depth > 0 && SwitcherEditorConfig.instance.bypassNestedSwitcherTree && current[0].TryGetComponent<Switcher>(out _);  // Transform is extracted only for the nested switcher
            Component[][] comps = null;
            if (bypassNestedSwitcher)
            {
                comps = current.ConvertAll(t => new Component[] { t.GetComponent<Transform>()});
            } else
            {
                comps = current.ConvertAll(t => t.GetComponents<Component>().FindAll(c=>
                {
                    var switcherCondition = !(c is Switcher) || (SwitcherEditorConfig.instance.recordNestedSwitcherData && depth != 0);
                    return switcherCondition;
                }).ToArray());
            }
            for (var i = 0; i < comps[0].Length; ++i)
            {
                GetMatchingComponentDiff(roots, comps, i, store);
            }
            foreach (var compDataList in store)
            {
                foreach (var c in compDataList)
                {
                    c.Postprocess(compDataList);
                }
            }

            // erase root position of RectTransform
            if (depth == 0 && !extractRootDiff)
            {
                foreach (var s in store)
                {
                    foreach (var d in s)
                    {
                        if (d is RectTransformData rtd && roots.IndexOf(r => r == d.target) >= 0)
                        {
                            rtd.anchoredPosition_mod = false;
                        }
                    }
                }
            }

            if (!bypassNestedSwitcher)
            {
                // child diff
                for (int i=0; i < current[0].GetSerializedChildCount(); ++i)
                {
                    var children = current.ConvertAll(p => p.GetSerializedChild(i));
                    GetDiffRecursively(roots, children, store, depth+1, extractRootDiff);
                }
            }
        }

        /// <summary>
        /// return Component data if all components' data are the same.
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="comps">return Component data if all components' data are the same.</param>
        private static void GetMatchingComponentDiff(Transform[] roots, Component[][] comps, int index, List<CompData>[] store)
        {
            var data = new CompData[comps.Length];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = CompDataFactory.instance.GetComponentData(comps[i][index]);
            }
            if (data[0] != null)
            {
                var diff = GetDiffs(data);
                if (diff)
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        data[i].target = data[0].target;
                        store[i].Add(data[i]);
                    }
                }
            }
        }

        internal static bool GetDiffs(IList<CompData> data)
        {
            var members = MemberControl.ListAttributedMembers(data[0].srcType, data[0].GetType(), null);
            var anyChanged = false;
            foreach (var m in members)
            {
                var collectIndex = new List<int>();
                for (var i=0; i<data.Count; ++i)
                {
                    if (data[i].IsCollectable(data[i].target, m))
                    {
                        collectIndex.Add(i);
                    }
                }

                var changed = false;
                if (collectIndex.Count > 1)
                {
                    var i0 = collectIndex[0];
                    for (int i=1; i<collectIndex.Count; ++i)
                    {
                        if (!data[i0].MemberEquals(data[collectIndex[i]], m))
                        {
                            changed = true;
                        }
                    }
                }
                else if (collectIndex.Count == 1)
                {
                    // 유효한 값이 하나일 때는 첫 component 에 할당한다.
                    var i = collectIndex[0];
                    var val = data[i].GetValue(m);
                    m.StoreValue(data[0], val);
                    m.Apply(data[0].target, val);
                }
                anyChanged |= changed;
                foreach (var d in data)
                {
                    m.SetChanged(d, changed && d.IsCollectable(d.target, m));
                }
            }
            return anyChanged;
        }

        internal static List<string> GetDuplicateSiblingNames(IReadOnlyList<GameObject> objs)
        {
            List<string> dup = new List<string>();
            foreach (var o in objs)
            {
                if (o != null && o.IsSerializable())
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
                    if (child.IsSerializable())
                    {
                        if (!names.Add(child.name))
                        {
                            d.Add(child.name);
                        }
                        d.AddRange(GetDuplicateSiblingNames(child));
                    }
                }
                return d;
            }
        }

        internal static bool IsChildrenMatches(IReadOnlyList<Transform> objs)
        {
            int childCount = objs[0].GetSerializedChildCount();
            for (int i = 1; i < objs.Count; ++i)
            {
                if (objs[i].GetSerializedChildCount() != childCount)
                {
                    return false;
                }
            }
            Transform[] children = new Transform[objs.Count];
            for (int c=0; c<childCount; ++c)
            {
                for (int i=0; i<objs.Count; ++i)
                {
                    children[i] = objs[i].GetSerializedChild(c);
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

        internal static bool IsComponentMatch(IReadOnlyList<Transform> objs)
        {
            var passed = true;
            var comps = new List<Component>[objs.Count];
            var count = -1;
            for (int i = 0; i < objs.Count; ++i)
            {
                comps[i] = SwitcherEditorConfig.instance.recordNestedSwitcherData?
                    objs[i].GetComponents<Component>().ToList():
                    objs[i].GetComponents<Component>().FindAll(co => !(co is Switcher));
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
            for (int c=0; c<objs[0].GetSerializedChildCount(); ++c)
            {
                List<Transform> children = new List<Transform>();
                for (int i=0; i<objs.Count; ++i)
                {
                    children.Add(objs[i].GetSerializedChild(c));
                }
                passed &= IsComponentMatch(children);
            }
            return passed;
        }

        internal static int GetSiblingIndex(string objName, Transform parent)
        {
            for (int i = 0; i < parent.GetSerializedChildCount(); ++i)
            {
                var c = parent.GetSerializedChild(i);
                if (c.name == objName)
                {
                    return i;
                }
            }
            return -1;
        }

        internal static List<T>[] FindAll<T>(List<CompData>[] diffs) where T : CompData
        {
            var list = new List<T>[diffs.Length];
            for (int i=0; i<diffs.Length; ++i)
            {
                list[i] = diffs[i].FindAll(c => c is T).ConvertAll(t => (T)t);
            }
            return list;
        }
    }

    class RootNTransform : RnC<Transform>
    {
        internal RootNTransform(Transform root, Transform child) : base(root, child) { }
    }

    class RootNComp : RnC<Behaviour>
    {
        internal RootNComp(Transform root, Behaviour child) : base(root, child) { }
    }

    class RnC<T> where T: Component
    {
        internal readonly Transform root;
        internal readonly T comp;

        internal string name => comp.name;
        internal Type type => comp.GetType();

        internal RnC(Transform root, T child)
        {
            this.root = root;
            this.comp = child;
        }

    }
}