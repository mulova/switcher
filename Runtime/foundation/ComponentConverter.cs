//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [ExecuteInEditMode]
    public class ComponentConverter<S, D> : MonoBehaviour where S : Component where D : Component
    {
#if UNITY_EDITOR
        public static readonly BindingFlags FIELD_FLAG = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy) & ~BindingFlags.SetProperty & ~BindingFlags.GetProperty;
        [SerializeField, HideInInspector] private bool isExecuted; // for undo

        protected virtual void Awake()
        {
            var executeCache = isExecuted;
            isExecuted = true;
            UnityEditor.Undo.SetCurrentGroupName($"Replace {typeof(S).Name} -> {typeof(D).Name}");
            var group = UnityEditor.Undo.GetCurrentGroup();
            if (!executeCache)
            {
                ReplaceComponent(gameObject);
            }
            UnityEditor.Undo.DestroyObjectImmediate(this);
            UnityEditor.Undo.CollapseUndoOperations(group);
        }

        public static D ReplaceComponent(GameObject o)
        {
            var src = o.GetComponent<S>();
            var all = o.GetComponents<Component>();
            var compIndex = Array.IndexOf(all, src);
            var up = all.Length - 1 - compIndex;
            var type = typeof(S).IsAssignableFrom(typeof(D)) ? typeof(S) : typeof(D);
            var values = new List<object>();
            var allFields = new List<FieldInfo>();
            while (type != null && type != typeof(Object))
            {
                var fields = type.GetFields(FIELD_FLAG);
                foreach (var f in fields)
                {
                    values.Add(f.GetValue(src));
                    allFields.Add(f);
                }
                type = type.BaseType;
            }
            UnityEditor.Undo.DestroyObjectImmediate(src);
            var dst = UnityEditor.Undo.AddComponent<D>(o);
            if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(o))
            {
                for (int i = 0; i < up; ++i)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(dst);
                }
            }
            for (int i = 0; i < allFields.Count; ++i)
            {
                allFields[i].SetValue(dst, values[i]);
            }
            return dst;
        }
#endif
    }
}
