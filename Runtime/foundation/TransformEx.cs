//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System.Collections.Generic;
    using System.Text;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Assertions;

    public static class TransformEx
    {
        public static string GetScenePath(this Transform trans, int depth = int.MaxValue)
        {
            var result = trans.GetScenePath(null, depth);
            return result.hierarchy;
        }

        private static Stack<Transform> stack = new Stack<Transform>();
        private static StringBuilder strBuilder = new StringBuilder();
        public static (string hierarchy, string[] paths, int[] index) GetScenePath(this Transform trans, Transform root, int depth = int.MaxValue)
        {
            lock (stack)
            {
                stack.Clear();
                strBuilder.Clear();
                Transform t = trans;
                var d = depth;
                while (t != null && t != root && d > 0)
                {
                    stack.Push(t);
                    t = t.parent;
                    --d;
                }
                var paths = new string[stack.Count];
                var index = new int[stack.Count];
                int i = 0;
                if (d > 0 && depth != int.MaxValue)
                {
                    strBuilder.Append("...");
                }
                while (stack.Count > 0)
                {
                    var s = stack.Pop();
                    paths[i] = s.name;
                    index[i] = s.GetSiblingIndex();
                    strBuilder.Append('/').Append(s.name);
                    i++;
                }
                return (strBuilder.ToString(), paths, index);
            }
        }

        public static Transform GetHierarchyPair(this Transform t, Transform root, Transform targetRoot)
        {
            if (t == root)
            {
                return targetRoot;
            }
            if (!t.IsChildOf(root))
            {
                return null;
            }
            var hierarchy = new List<int>();
            while (t != root && t != null)
            {
                hierarchy.Add(t.GetSiblingIndex());
                t = t.parent;
            }
            hierarchy.Reverse();
            var ret = targetRoot;
            foreach (var i in hierarchy)
            {
                if (i < ret.childCount)
                {
                    ret = ret.GetChild(i);
                }
                else
                {
                    return null;
                }
            }
            return ret;
        }

        public static bool IsHierarchyPair(this Transform t1, Transform t2)
        {
            if (t1 == t2)
            {
                return true;
            }
            else if (t1 != null ^ t2 != null)
            {
                return false;
            }
            else if (t1.GetSiblingIndex() != t2.GetSiblingIndex())
            {
                return false;
            }
            else if (t1.parent == t2.parent) // root doesn't need to be the same transform
            {
                return true;
            }
            else
            {
                return IsHierarchyPair(t1.parent, t2.parent);
            }
        }

        public static bool IsSerializable(this Transform t) => t.gameObject.IsSerializable();

        public static bool IsSerializable(this GameObject o)
        {
            return (o.hideFlags & HideFlags.HideAndDontSave) == 0 && !o.TryGetComponent<TMP_SubMeshUI>(out var _);
        }

        public static List<Transform> GetSerializableChildren(this Transform parent)
        {
            var list = new List<Transform>();
            foreach (Transform c in parent)
            {
                if (c.IsSerializable())
                {
                    list.Add(c);
                }
            }
            return list;
        }

        public static int GetSerializedChildCount(this Transform parent)
        {
            var count = 0;
            foreach (Transform c in parent)
            {
                if (c.IsSerializable())
                {
                    ++count;
                }
            }
            return count;
        }

        public static Transform GetSerializedChild(this Transform parent, int i)
        {
            var count = 0;
            foreach (Transform c in parent)
            {
                if (c.IsSerializable())
                {
                    if (count == i)
                    {
                        return c;
                    }
                    ++count;
                }
            }
            return null;
        }
    }
}
