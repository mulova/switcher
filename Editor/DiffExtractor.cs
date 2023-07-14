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
        internal static void ReplaceRefs(Transform trans, Transform rc, Transform r0)
        {
            var comps = trans.GetComponents<Component>();
            foreach (var c in comps)
            {
                ReplaceRefs(c, rc, r0);
            }
            // Recursive call
            foreach (Transform child in trans)
            {
                ReplaceRefs(child, rc, r0);
            }
        }

        private static void ReplaceRefs(Component c, Transform rc, Transform r0)
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

        internal static void CreateMissingChildren(IList<Transform> parents, IList<Transform> roots)
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
                        var clone = CloneSibling(c.comp, p, insertIndex);
                        ReplaceRefs(clone, c.root, roots[ip]);
                        clone.gameObject.SetActive(false);
                        ++insertIndex;
                    }
                }
            }

            // sort children
            for (int i=0; i< parents[0].childCount; ++i)
            {
                var c = parents[0].GetChild(i);

                var childRoots = new List<Transform>();
                for (int j=0; j < parents.Count; ++j)
                {
                    childRoots.Add(parents[j].GetChild(i));
                    if (j != 0)
                    {
                        parents[j].Find(c.name).SetSiblingIndex(i);
                    }
                }
                CreateMissingChildren(childRoots, roots);
            }

            static List<RootNTransform> GetChildUnion(IList<Transform> parents, IList<Transform> roots)
            {
                Assert.AreEqual(parents.Count, roots.Count);
                List<RootNTransform> union = new List<RootNTransform>();
                for (int i = 0; i < parents[0].childCount; ++i)
                {
                    union.Add(new RootNTransform(roots[0], parents[0].GetChild(i)));
                }

                for (int ip = 1; ip < parents.Count; ++ip)
                {
                    var parent = parents[ip];
                    var oldIndex = -1;
                    for (int ic = 0; ic < parent.childCount; ++ic)
                    {
                        var c = parent.GetChild(ic);
                        int index = union.FindIndex(t => t.name == c.name);
                        if (index < 0)
                        {
                            // find next component index
                            for (var ic2 = ic + 1; ic2 < parent.childCount && index < 0; ++ic2)
                            {
                                index = union.FindIndex(t => t.name == parent.GetChild(ic2).name);
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

        internal static void CreateMissingComponent(IList<Transform> objs, IList<Transform> roots)
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
                        ReplaceRefs(clone, c.root, roots[ip]);
                        clone.enabled = false;
                        ++insertIndex;
                    }
                }
            }

            // sort children
            for (int i = 0; i < objs[0].childCount; ++i)
            {
                var c = objs[0].GetChild(i);

                var childRoots = new List<Transform>();
                for (int j = 0; j < objs.Count; ++j)
                {
                    childRoots.Add(objs[j].GetChild(i));
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
                var filtered = all.Where(c => c.GetType() != typeof(Switcher));
                return filtered.ToList();
            }

            static List<RootNComp> GetComponentUnion(IList<Transform> objs, IList<Transform> roots)
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

        internal static List<ICompData>[] CreateDiff(GameObject[] roots)
        {
            var trans = roots.ConvertAll(o => o.transform);
            var store = trans.ConvertAll(p => new List<ICompData>());
            GetDiffRecursively(trans, trans, store, 0);
            return store;
        }

        private static void GetDiffRecursively(Transform[] roots, Transform[] current, List<ICompData>[] store, int depth)
        {
            if (roots[0].TryGetComponent<IgnoreSwitch>(out _) || (depth > 0 && current[0].TryGetComponent<Switcher>(out _))) // No diff extracted for the child switcher
            {
                return;
            }
            var comps = current.ConvertAll(p => p.GetComponents<Component>().FindAll(c=> !(c is Switcher) && (depth != 0 || !(c is Transform))).ToArray());
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

        internal static bool GetDiffs(ICompData[] data)
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

        internal static List<string> GetDuplicateSiblingNames(IList<GameObject> objs)
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
                    if ((child.gameObject.hideFlags & HideFlags.HideAndDontSave) != 0)
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

        internal static bool IsChildrenMatches(IList<Transform> objs)
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

        internal static bool IsComponentMatch(IList<Transform> objs)
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

        internal static int GetSiblingIndex(string objName, Transform parent)
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