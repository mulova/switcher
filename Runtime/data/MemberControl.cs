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
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Events;

    [UnityEngine.Scripting.Preserve]
    public class MemberControl
    {
        private readonly FieldInfo srcField;
        private readonly PropertyInfo srcProperty;
        private readonly FieldInfo storeField;
        private readonly FieldInfo storeModField;
        public string name => storeField.Name;
        public bool isCustom => srcField == null && srcProperty == null;
        public bool isReference => typeof(Component).IsAssignableFrom(storeField.FieldType);
        public Type memberType => storeField.FieldType;

        public static readonly BindingFlags INSTANCE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        public static readonly BindingFlags FIELD_FLAG = INSTANCE_FLAGS & ~BindingFlags.SetProperty & ~BindingFlags.GetProperty;
        public static readonly BindingFlags PROPERTY_FLAG = INSTANCE_FLAGS & ~BindingFlags.GetField & ~BindingFlags.SetField;
        public const string MOD_SUFFIX = "_mod";

        private static ILogger log => null;
        //private static ILogger log => Debug.unityLogger;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            cache = null;
        }

        public MemberControl(FieldInfo storeField, FieldInfo storeModField)
        {
            this.storeField = storeField;
            this.storeModField = storeModField;
        }

        public MemberControl(FieldInfo storeField, FieldInfo storeModField, MemberInfo member)
        {
            this.storeField = storeField;
            this.storeModField = storeModField;
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
            return storeModField == null || (bool)storeModField.GetValue(store);
        }

        public void SetChanged(ICompData store, bool changed)
        {
            storeModField?.SetValue(store, changed);
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
                var isMod = storeModField == null || (bool)storeModField.GetValue(store);
                if (!isMod)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2} is not set", target.name, storeModField.DeclaringType.Name, storeModField.Name);
                    return;
                }
                var val = storeField.GetValue(store);
                if (storeModField != null)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2} stored value: {3}", target.name, storeModField.DeclaringType.Name, storeModField.Name, val);
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

        private static Dictionary<Type, List<MemberControl>> cache;

        public static List<MemberControl> ListAttributedMembers(Type srcType, Type storeType, Action<List<MemberControl>> sorter)
        {
            if (cache == null)
            {
                cache = new Dictionary<Type, List<MemberControl>>();
            }
            if (!cache.TryGetValue(srcType, out var list))
            {
                list = new List<MemberControl>();
                var storeFields = storeType.GetFields(FIELD_FLAG);
                foreach (FieldInfo f in storeFields)
                {
                    var a = f.GetCustomAttribute<StoreAttribute>();
                    if (a != null)
                    {
                        var isModField = storeType.GetField(f.Name + MOD_SUFFIX, FIELD_FLAG);
                        if (isModField != null && isModField.FieldType == typeof(bool))
                        {
                            var member = srcType.GetMember(f.Name, INSTANCE_FLAGS);
                            var slot = new MemberControl(f, isModField, member.FirstOrDefault());
                            list.Add(slot);
                        }
                        else
                        {
                            var member = srcType.GetMember(f.Name, INSTANCE_FLAGS);
                            var slot = new MemberControl(f, isModField, member.FirstOrDefault());
                            list.Add(slot);
                            log?.LogFormat(LogType.Log, "{0}.{1}{2} field is missing", f.DeclaringType.Name, f.Name, MOD_SUFFIX);
                        }
                    }
                }
                sorter?.Invoke(list);
                cache[srcType] = list;
            }
            return list;
        }
    }
}

