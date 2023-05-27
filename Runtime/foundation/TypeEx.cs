//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher{

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public static class TypeEx
	{
        public static readonly BindingFlags INSTANCE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        public static readonly BindingFlags VAR_FLAGS = INSTANCE_FLAGS & ~BindingFlags.SetProperty & ~BindingFlags.GetProperty;
        private static ILogger log => UnityEngine.Debug.unityLogger;

        public static List<MethodInfo> ListMethods(this Type type, ICollection<Type> excludes)
        {
            return type.ListMethods(VAR_FLAGS, excludes);
        }

        public static List<FieldInfo> ListFields(this Type type, BindingFlags flags, ICollection<Type> excludes)
		{
            List<FieldInfo> fields = new List<FieldInfo>();
            HashSet<Type> excludeSet = excludes != null ? new HashSet<Type>(excludes) : null;
            foreach (FieldInfo f in type.GetFields(flags))
            {
                if (excludeSet == null || !excludeSet.Contains(f.DeclaringType))
                {
                    fields.Add(f);
                }
            }
            fields.Sort(CompareFields);
            return fields;

            int CompareFields(FieldInfo x, FieldInfo y)
            {
                if (x != null)
                {
                    if (y == null)
                    {
                        return -1;
                    }
                    else
                    {
                        return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                    }
                }
                return 1;
            }
        }

        public static List<PropertyInfo> ListProperty(this Type type, BindingFlags flags, ICollection<Type> excludes)
        {
            List<PropertyInfo> props = new List<PropertyInfo>();
            HashSet<Type> excludeSet = excludes != null ? new HashSet<Type>(excludes) : null;
            foreach (PropertyInfo p in type.GetProperties(flags))
            {
                if (excludeSet == null || !excludeSet.Contains(p.DeclaringType))
                {
                    props.Add(p);
                }
            }
            props.Sort(CompareProperties);
            return props;

            int CompareProperties(PropertyInfo x, PropertyInfo y)
            {
                if (x != null)
                {
                    if (y == null)
                    {
                        return -1;
                    }
                    else
                    {
                        return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                    }
                }
                return 1;
            }
        }

        public static List<MethodInfo> ListMethods(this Type type, BindingFlags flags, ICollection<Type> excludes)
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            HashSet<Type> excludeSet = excludes != null ? new HashSet<Type>(excludes) : null;
            foreach (MethodInfo m in type.GetMethods(flags))
            {
                if (excludeSet == null || !excludeSet.Contains(m.DeclaringType) && !m.IsSpecialName)
                {
                    methods.Add(m);
                }
            }
            methods.Sort(CompareMethods);
            return methods;

            int CompareMethods(MethodInfo x, MethodInfo y)
            {
                if (x != null)
                {
                    if (y == null)
                    {
                        return -1;
                    }
                    else
                    {
                        return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                    }
                }
                return 1;
            }
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            foreach (Attribute attr in type.GetCustomAttributes(true))
            {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (attr.TypeId == typeof(T))
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                {
                    return attr as T;
                }
            }
            return null;
        }

        public static List<Type> FindTypes(this Type type)
        {
            List<Type> found = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    foreach (Type t in assembly.GetTypes())
                    {
                        if (type.IsAssignableFrom(t))
                        {
                            found.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogException(ex);
                }
            }
            return found;
        }

        private static int assemblyCounts;
        private static Dictionary<string, Type> types;

        public static Type GetType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblyCounts != assemblies.Length)
            {
                assemblyCounts = assemblies.Length;
                types = new Dictionary<string, Type>();
                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        types[type.FullName] = type;
                    }
                }
            }
            if (types.TryGetValue(fullName, out var t))
            {
                return t;
            } else
            {
                return default;
            }
        }

        private static MultiMap<Type, Type> attrTypes;

        public static List<Type> FindTypesWithAttribute<T>() where T : Attribute
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (attrTypes == null)
            {
                attrTypes = new MultiMap<Type, Type>();
            }
            if (!attrTypes.ContainsKey(typeof(T)))
            {
                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                        {
                            attrTypes.Add(typeof(T), type);
                        }
                    }
                }
            }
            return attrTypes[typeof(T)];
        }
    }

}

