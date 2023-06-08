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
    using UnityEngine.Events;

    [UnityEngine.Scripting.Preserve]
    public class MemberControl
    {
        private readonly FieldInfo srcField;
        private readonly PropertyInfo srcProperty;
        private readonly FieldInfo storeField;
        private readonly FieldInfo storeIsSetField;
        public string name => storeField.Name;
        public bool isCustom => srcField == null && srcProperty == null;
        public bool isReference => typeof(Component).IsAssignableFrom(storeField.FieldType);

        private ILogger log => null;
        //private ILogger log => Debug.unityLogger;

        public Type memberType => storeField.FieldType;

        public MemberControl(FieldInfo storeField, FieldInfo storeIsSetField)
        {
            this.storeField = storeField;
            this.storeIsSetField = storeIsSetField;
        }

        public MemberControl(FieldInfo storeField, FieldInfo storeIsSetField, MemberInfo member)
        {
            this.storeField = storeField;
            this.storeIsSetField = storeIsSetField;
            switch (member)
            {
                case FieldInfo f:
                    srcField = f;
                    break;
                case PropertyInfo p:
                    srcProperty = p;
                    break;
            }
        }

        public bool HasChanged(ICompData store)
        {
            return storeIsSetField == null || (bool)storeIsSetField.GetValue(store);
        }

        public void SetChanged(ICompData store, bool changed)
        {
            storeIsSetField?.SetValue(store, changed);
        }

        public bool IsTypeOf(Type type) => type.IsAssignableFrom(memberType);

        public object GetValue(Component c)
        {
            try
            {
                if (srcField != null)
                {
                    return srcField.GetValue(c);
                }
                else if (srcProperty != null)
                {
                    return srcProperty.GetValue(c);
                }
                else
                {
                    throw new Exception($"No getter member for '{c.GetType().Name}.{storeField.Name}'");
                }
            } catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
                throw;
            }
        }

        public void SetValue(ICompData store, object val)
        {
            try
            {
                storeField.SetValue(store, val);
            } catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="src"></param>
        /// <param name="rc">root of the current case</param>
        /// <param name="r0">root of the first case</param>
        public void Collect(ICompData store, Component src, Transform rc, Transform r0)
        {
            try
            {
                var val = GetValue(src);
                if (val != null && rc != r0)
                {
                    if (IsTypeOf(typeof(UnityEventBase)))
                    {
                        var e = val as UnityEventBase;
                        e.ReplaceMatchingRoot(rc, r0);
                    } else if (IsTypeOf(typeof(Component)))
                    {
                        var c = val as Component;
                        var match = GetComponentMatch(c, rc, r0);
                        if (match != null)
                        {
                            val = match;
                        }
                    } else if (IsTypeOf(typeof(GameObject)))
                    {
                        var o = val as GameObject;
                        var t = o.transform;
                        var match = GetTransformMatch(t, rc, r0);
                        if (match != null)
                        {
                            val = match.gameObject;
                        }
                    }
                }
                SetValue(store, val);
            }
            catch {
                Debug.LogErrorFormat("{0}.{1}", storeField.ReflectedType.FullName, storeField.Name);
                throw;
            }
        }

        public object GetValue(ICompData store)
        {
            try
            {
                return storeField.GetValue(store);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
                throw;
            }
        }

        public void Apply(Component target, CompData store)
        {
            if (target == null)
            {
                return;
            }
            try
            {
                var isSet = storeIsSetField == null || (bool)storeIsSetField.GetValue(store);
                if (!isSet)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2} is not set", target.name, storeIsSetField.DeclaringType.Name, storeIsSetField.Name);
                    return;
                }
                var val = storeField.GetValue(store);
                if (storeIsSetField != null)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2} stored value: {3}", target.name, storeIsSetField.DeclaringType.Name, storeIsSetField.Name, val);
                }
                if (srcField != null)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2}={3}", target.name, srcField.DeclaringType.Name, srcField.Name, val);
                    srcField.SetValue(target, val);
                }
                else if (srcProperty != null)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2}={3}", target.name, srcProperty.DeclaringType.Name, srcProperty.Name, val);
                    srcProperty.SetValue(target, val);
                }
                else
                {
                    throw new Exception($"'{storeField.Name}' is custom field. Override CompData.Collect(), CompData.Apply() methods");
                }
            } catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
            }
        }

        public override string ToString()
        {
            return $"{memberType.Name}.{name}";
        }

        public static Component GetComponentMatch(Component c, Transform root, Transform targetRoot)
        {
            var match = GetTransformMatch(c.transform, root, targetRoot);
            if (match != null)
            {
                return match.GetComponent(c.GetType());
            } else
            {
                return null;
            }
        }

        public static Transform GetTransformMatch(Transform t, Transform root, Transform targetRoot)
        {
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
                } else
                {
                    return null;
                }
            }
            return ret;
        }

        public static bool IsHierarchyMatch(Transform t1, Transform t2)
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
                return IsHierarchyMatch(t1.parent, t2.parent);
            }
        }
    }
}

