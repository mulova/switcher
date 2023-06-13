//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using Object = UnityEngine.Object;

    internal class CompDataFactory
    {
        internal static CompDataFactory instance = new CompDataFactory();
        private Dictionary<Type, Type> pool;

        public ICompData GetComponentData(Component c)
        {
            var dataType = FindDataType(c.GetType());
            if (dataType != null)
            {
                var o = Activator.CreateInstance(dataType) as ICompData;
                o.Collect(c, false);
                return o;
            }
            else
            {
                return null;
            }
        }

        public Type FindDataType(Type type)
        {
            // collect BuildProcessors
            if (pool == null)
            {
                pool = new Dictionary<Type, Type>();
                List<Type> cls = typeof(ICompData).FindTypes();
                foreach (Type t in cls)
                {
                    if (!t.IsAbstract)
                    {
                        try
                        {
                            var ins = Activator.CreateInstance(t) as ICompData;
                            pool[ins.srcType] = t;
                        } catch
                        {
                            Debug.LogError($"Activator.CreateInstance({t.FullName}) failed.");
                            throw;
                        }
                    }
                }
            }
            pool.TryGetValue(type, out var dataType);
            var baseType = type.BaseType;
            while (dataType == null && (baseType != null && baseType != typeof(Object) && baseType != baseType.BaseType))
            {
                pool.TryGetValue(baseType, out dataType);
                if (dataType != null)
                {
                    pool[type] = dataType;
                    break;
                }
                baseType = baseType.BaseType;
            }
            return dataType;
        }
    }
}