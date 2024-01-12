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
        internal readonly FieldInfo storeField;
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

        public bool HasChanged(CompData store)
        {
            return storeModField == null || (bool)storeModField.GetValue(store);
        }

        public void SetChanged(CompData store, bool changed)
        {
            storeModField?.SetValue(store, changed);
        }

        public bool IsTypeOf(Type type) => type.IsAssignableFrom(memberType);

        public object GetValue(object src)
        {
            try
            {
                if (srcField != null)
                {
                    return srcField.GetValue(src);
                }
                else if (srcProperty != null)
                {
                    return srcProperty.GetValue(src);
                }
                else
                {
                    throw new Exception($"No getter member for '{src.GetType().Name}.{storeField.Name}'");
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", storeField.ReflectedType.FullName, storeField.Name, ex);
                throw;
            }
        }

        public void StoreValue(CompData store, object val)
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
                Apply(target, val);
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
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2}={3}", target.name, srcField.DeclaringType.Name, srcField.Name, value);
                    srcField.SetValue(target, value);
                }
                else if (srcProperty != null)
                {
                    log?.LogFormat(LogType.Log, target, "[{0}] {1}.{2}={3}", target.name, srcProperty.DeclaringType.Name, srcProperty.Name, value);
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

        public static List<MemberControl> ListAttributedMembers(Type srcType, Type storeType, IReadOnlyList<string> memberOrder)
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
                            var slot = new MemberControl(f, isModField, a);
                            list.Add(slot);
                            //Debug.LogFormat("Custom member {0}.{1}", srcType.FullName, f.Name);
                        }
                    }
                }
                if (memberOrder != null)
                {
                    Sort(list, memberOrder);
                }
                cache[srcType] = list;
            }
            return list;

            static void Sort(List<MemberControl> members, IReadOnlyList<string> order)
            {
                members.Sort((a, b) =>
                {
                    foreach (var o in order)
                    {
                        if (a.name == o)
                        {
                            return -1;
                        }
                        else if (b.name == o)
                        {
                            return 1;
                        }
                    }
                    return a.name.CompareTo(b.name);
                });
            }
        }
    }
}

