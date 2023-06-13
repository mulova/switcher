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
        public readonly StoreAttribute attr;

        private static ILogger log => null;
        //private static ILogger log => Debug.unityLogger;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            cache = null;
        }

        public MemberControl(FieldInfo storeField, FieldInfo storeModField, StoreAttribute attr)
        {
            this.storeField = storeField;
            this.storeModField = storeModField;
            this.attr = attr;
        }

        public MemberControl(MemberInfo member, FieldInfo storeField, FieldInfo storeModField, StoreAttribute attr) : this(storeField, storeModField, attr)
        {
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
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
                throw;
            }
        }

        public void StoreValue(ICompData store, object val)
        {
            try
            {
                storeField.SetValue(store, val);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="src"></param>
        public void Collect(ICompData store, Component src)
        {
            try
            {
                var val = GetValue(src);
                StoreValue(store, val);
            }
            catch
            {
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
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
            }
        }

        public void Apply(Component target, object value)
        {
            if (target == null)
            {
                return;
            }
            try
            {
                if (srcField != null)
                {
                    srcField.SetValue(target, value);
                }
                else if (srcProperty != null)
                {
                    srcProperty.SetValue(target, value);
                }
                else
                {
                    throw new Exception($"{storeField.ReflectedType.FullName}.{storeField.Name}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
            }
        }

        public override string ToString()
        {
            return $"{memberType.Name}.{name}";
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
                        if (isModField == null)
                        {
                            Debug.LogErrorFormat("{0}.{1}{2} field is missing", f.DeclaringType.FullName, f.Name, MOD_SUFFIX);
                        }
#if UNITY_EDITOR
                        if (!Application.isPlaying && isModField != null)
                        {
                            if (isModField.FieldType != typeof(bool))
                            {
                                throw new MissingFieldException($"Type of '{f.DeclaringType.FullName}.{f.Name}' is not bool");
                            }
                            if (isModField.GetCustomAttribute<HideInInspector>() == null)
                            {
                                Debug.LogError($"'{f.DeclaringType.FullName}.{f.Name}' needs [HideInInspector]");
                            }
                        }
#endif
                        if (a.manual)
                        {
                            var slot = new MemberControl(f, isModField, a);
                            list.Add(slot);
                        }
                        else
                        {
                            var member = srcType.GetMember(f.Name, INSTANCE_FLAGS);
                            if (member.Length > 0)
                            {
                                if (member[0] is PropertyInfo p && (!p.CanRead || !p.CanWrite))
                                {
                                    Debug.LogErrorFormat("{0}.{1} is not readable / writable", srcType.FullName, f.Name);
                                }
                                else
                                {
                                    var slot = new MemberControl(member[0], f, isModField, a);
                                    list.Add(slot);
                                }
                            }
                            else
                            {
                                Debug.LogErrorFormat("{0}.{1} field/property is missing", srcType.FullName, f.Name);
                            }
                        }
                    }
                }
                sorter?.Invoke(list);
                cache[srcType] = list;
            }
            return list;
        }

        /// <param name="rc">root of the current case</param>
        /// <param name="r0">root of the first case</param>
        public void ReplaceRefs(Component comp, Transform rc, Transform r0)
        {
            try
            {
                var val = GetValue(comp);
                if (val != null && rc != r0)
                {
                    if (IsTypeOf(typeof(UnityEventBase)))
                    {
                        var e = val as UnityEventBase;
                        e.ReplaceMatchingTarget(rc, r0);
                    }
                    else if (IsTypeOf(typeof(Component)))
                    {
                        var c = val as Component;
                        var match = c.GetHierarchyPair(rc, r0);
                        if (match != null)
                        {
                            val = match;
                        }
                    }
                    else if (IsTypeOf(typeof(GameObject)))
                    {
                        var o = val as GameObject;
                        var t = o.transform;
                        var match = t.GetHierarchyPair(rc, r0);
                        if (match != null)
                        {
                            val = match.gameObject;
                        }
                    }
                }

                Apply(comp, val);
            }
            catch
            {
                Debug.LogErrorFormat("{0}.{1}", storeField.ReflectedType.FullName, storeField.Name);
                throw;
            }
        }
    }
}

