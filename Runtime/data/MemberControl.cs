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
                log?.LogException(ex);
                throw;
            }
        }

        public void SetValue(ICompData store, object val)
        {
            try
            {
                storeField.SetValue(store, val);
            } catch
            {
                log?.LogFormat(LogType.Error, "{0}.{1}", storeField.ReflectedType.FullName, storeField.Name);
                throw;
            }
        }

        public void Collect(ICompData store, Component src)
        {
            try
            {
                storeField.SetValue(store, GetValue(src));
            }
            catch {
                log?.LogFormat(LogType.Error, "{0}.{1}", storeField.ReflectedType.FullName, storeField.Name);
                throw;
            }
        }

        public object GetValue(ICompData store)
        {
            try
            {
                return storeField.GetValue(store);
            }
            catch
            {
                log?.LogFormat(LogType.Error, "{0}.{1}", storeField.ReflectedType.FullName, storeField.Name);
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
                log?.LogException(ex);
                throw;
            }
        }

        public override string ToString()
        {
            return name;
        }

        public static Transform GetMatchingTransform(Transform t, Transform root, Transform matchingRoot)
        {
            var hierarchy = new List<int>();
            while (t != root)
            {
                hierarchy.Add(t.GetSiblingIndex());
                t = t.parent;
            }
            hierarchy.Reverse();
            var ret = matchingRoot;
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
            else if (t1.parent == t2.parent) // root doesn't need to be the same transform
            {
                return true;
            }
            else if (t1.GetSiblingIndex() != t2.GetSiblingIndex())
            {
                return false;
            }
            else
            {
                return IsHierarchyMatch(t1.parent, t2.parent);
            }
        }
    }
}

