//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher.foundation
{
    using UnityEngine;
    using UnityEngine.Events;
    using Object = UnityEngine.Object;

    public static class UnityEventEx
    {
        public static bool HasPersistentCall(this UnityEventBase e, string methodName)
        {
            for (int i=0; i< e.GetPersistentEventCount(); ++i)
            {
                if (e.GetPersistentMethodName(i) == methodName)
                {
                    return true;
                }
            }
            return false;
        }

        public static object GetPersistentCall(this UnityEventBase e, int i)
        {
            var calls = typeof(UnityEventBase).GetField("m_PersistentCalls", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var m_PersistentCalls = calls.GetValue(e);
            var listener = m_PersistentCalls.GetType().GetMethod("GetListener", new[] { typeof(int) }).Invoke(m_PersistentCalls, new object[] { i });
            return listener;
        }

        public static void SetPersistentTarget(object call, Object target)
        {
            call.GetType().GetField("m_Target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(call, target);
        }

        public static bool IsArgumentEquals(object call1, object call2)
        {
            var arg1 = GetArgument(call1);
            var arg2 = GetArgument(call2);
            var sProp = arg1.GetType().GetProperty("stringArgument", typeof(string));
            var s1 = sProp.GetValue(arg1) as string;
            var s2 = sProp.GetValue(arg2) as string;
            if (s1 != s2)
            {
                return false;
            }
            var iProp = arg1.GetType().GetProperty("intArgument", typeof(int));
            if ((int)iProp.GetValue(arg1) != (int)iProp.GetValue(arg2))
            {
                return false;
            }
            var oProp = arg1.GetType().GetProperty("unityObjectArgument", typeof(Object));
            if ((Object)oProp.GetValue(arg1) != (Object)oProp.GetValue(arg2))
            {
                return false;
            }
            var fProp = arg1.GetType().GetProperty("floatArgument", typeof(float));
            if ((float)fProp.GetValue(arg1) != (float)fProp.GetValue(arg2))
            {
                return false;
            }
            var bProp = arg1.GetType().GetProperty("boolArgument", typeof(bool));
            if ((bool)bProp.GetValue(arg1) != (bool)bProp.GetValue(arg2))
            {
                return false;
            }
            return true;
        }

        public static object GetArgument(object call)
        {
            return call.GetType().GetField("m_Arguments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(call);
        }

        public static bool PersistentEventEquals(this UnityEventBase u1, UnityEventBase u2)
        {
            if (u2 == null)
            {
                return false;
            }
            if (u1.GetPersistentEventCount() != u2.GetPersistentEventCount())
            {
                return false;
            }
            for (int i = 0; i < u1.GetPersistentEventCount(); ++i)
            {
                if (u1.GetPersistentMethodName(i) != u2.GetPersistentMethodName(i))
                {
                    return false;
                }
                var t1 = u1.GetPersistentTarget(i) as Component;
                var t2 = u2.GetPersistentTarget(i) as Component;
                if (t1 != t2)
                {
                    if (t1 != null ^ t2 != null)
                    {
                        return false;
                    }
                    if (t1 != null && (t1.GetType() != t2.GetType() || t1.name != t2.name || !t1.transform.IsHierarchyPair(t2.transform)))
                    {
                        return false;
                    }
                }
                else
                {
                    // check arguments
                    var call0 = u1.GetPersistentCall(i);
                    var calli = u2.GetPersistentCall(i);
                    if (!IsArgumentEquals(call0, calli))
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public static void ReplaceMatchingTarget(this UnityEventBase evt, Transform root, Transform matchingRoot)
        {
            if (matchingRoot == root)
            {
                return;
            }
            for (int i = 0; i < evt.GetPersistentEventCount(); ++i)
            {
                var target = evt.GetPersistentTarget(i);
                switch (target)
                {
                    case Component c:
                        if (c != null && (c.transform == root || c.transform.IsChildOf(root)))
                        {
                            // m_PersistentCalls.GetListener (index)?.target
                            object call = evt.GetPersistentCall(i);
                            var match = c.GetHierarchyPair(root, matchingRoot);
                            if (match != null)
                            {
                                SetPersistentTarget(call, match);
                            }
                        }
                        break;
                    case GameObject o:
                        if (o != null && (o.transform == root || o.transform.IsChildOf(root)))
                        {
                            // m_PersistentCalls.GetListener (index)?.target
                            object call = evt.GetPersistentCall(i);
                            var match = o.transform.GetHierarchyPair(root, matchingRoot);
                            if (match != null)
                            {
                                SetPersistentTarget(call, match.gameObject);
                            }
                        }
                        break;
                }
                
            }
        }
    }

}

