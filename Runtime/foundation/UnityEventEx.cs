//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEngine;
    using UnityEngine.Events;
    using Object = UnityEngine.Object;

    public static class UnityEventEx
    {
        public static object GetPersistentCall(this UnityEvent e, int i)
        {
            var calls = typeof(UnityEventBase).GetField("m_PersistentCalls", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var m_PersistentCalls = calls.GetValue(e);
            var listener = m_PersistentCalls.GetType().GetMethod("GetListener", new[] { typeof(int) }).Invoke(m_PersistentCalls, new object[] { i });
            return listener;
        }

        public static void SetPersistentTarget(object call, int i, Component target)
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
    }

}

