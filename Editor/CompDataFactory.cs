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
    using UnityEngine.Assertions;

    public class CompDataFactory
    {
        public static CompDataFactory instance { get; private set; } = new CompDataFactory();
        private Dictionary<Type, Type> pool;
        private ILogger log => Debug.unityLogger;

        public CompData GetComponentData(Component c)
        {
            var dataType = FindDataType(c.GetType());
            if (dataType != null)
            {
                var o = Activator.CreateInstance(dataType) as CompData;
                o.Collect(c, false);
                return o;
            }
            else
            {
                return null;
            }
        }

        public void RegisterCustomDataType(Type compType, Type dataType)
        {
            Assert.IsTrue(compType.IsAssignableFrom(typeof(CompData)));
            Assert.IsTrue(dataType.IsAssignableFrom(typeof(Component)));
            pool[compType] = dataType;
        }

        public Type FindDataType(Type compType)
        {
            // collect BuildProcessors
            if (pool == null)
            {
                pool = new Dictionary<Type, Type>();
                List<Type> cls = typeof(CompData).FindTypes();
                foreach (Type t in cls)
                {
                    if (!t.IsAbstract)
                    {
                        try
                        {
                            var inst = Activator.CreateInstance(t) as CompData;
                            if (pool.TryGetValue(inst.srcType, out var oldType))
                            {
                                var oldInst = Activator.CreateInstance(oldType) as CompData;
                                if (inst.priority > oldInst.priority)
                                {
                                    pool[inst.srcType] = t;
                                    log.LogFormat(LogType.Log, "{0} is replaced by {1}", oldType.FullName, t.FullName);
                                }
                            } else
                            {
                                pool[inst.srcType] = t;
                            }
                        } catch
                        {
                            Debug.LogError($"Activator.CreateInstance({t.FullName}) failed.");
                            throw;
                        }
                    }
                }
            }
            pool.TryGetValue(compType, out var dataType);
            var baseType = compType.BaseType;
            while (dataType == null && (baseType != null && baseType != typeof(Object) && baseType != baseType.BaseType))
            {
                pool.TryGetValue(baseType, out dataType);
                if (dataType != null)
                {
                    pool[compType] = dataType;
                    break;
                }
                baseType = baseType.BaseType;
            }
            return dataType;
        }
    }
}