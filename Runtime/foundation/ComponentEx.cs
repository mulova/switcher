//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEngine;
    using UnityEngine.Assertions;

    public static class ComponentEx
    {
        public static Component GetHierarchyPair(this Component c, Transform root, Transform targetRoot)
        {
            var match = c.transform.GetHierarchyPair(root, targetRoot);
            if (match != null)
            {
                return match.GetComponent(c.GetType());
            }
            else
            {
                return null;
            }
        }
    }
}
