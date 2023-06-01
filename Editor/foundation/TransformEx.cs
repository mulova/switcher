//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

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
    }
}
