//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Scripting;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

    [Serializable, Preserve]
    public class ButtonData : SelectableData<Button>
    {
        [Store, Preserve] public Button.ButtonClickedEvent onClick;
        [HideInInspector, Preserve] public bool onClick_IsSet;

        protected override void CollectMember(MemberControl m, Component src, Transform rc, Transform r0)
        {
#if UNITY_EDITOR
            if (r0 != rc && m.IsTypeOf(typeof(UnityEvent)))
            {
                if (m.GetValue(src) is UnityEvent e)
                {
                    for (int i = 0; i < e.GetPersistentEventCount(); ++i)
                    {
                        var t = e.GetPersistentTarget(i) as Component;
                        if (t != null && t.transform.IsChildOf(rc))
                        {
                            // m_PersistentCalls.GetListener (index)?.target
                            object call = e.GetPersistentCall(i);
                            var r0Target = MemberControl.GetMatchingTransform(t.transform, rc, r0).GetComponent(t.GetType());
                            if (r0Target != null)
                            {
                                UnityEventEx.SetPersistentTarget(call, i, r0Target);
                                //listener.GetType().GetField("m_Target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(listener, r0Target); // call.target = val
                            }
                        }
                    }
                    m.SetValue(this, e);
                }
                 else
                {
                    m.SetValue(this, null);
                }
            } else
            {
                base.CollectMember(m, src, rc, r0);
            }
#endif
        }

        public override bool MemberEquals(MemberControl m, object val0, object vali)
        {
            if (m.IsTypeOf(typeof(UnityEvent)))
            {
                var e0 = val0 as UnityEvent;
                var ei = vali as UnityEvent;
                if (ei == null)
                {
                    return false;
                }
                if (e0.GetPersistentEventCount() != ei.GetPersistentEventCount())
                {
                    return false;
                }
                for (int i = 0; i < onClick.GetPersistentEventCount(); ++i)
                {
                    if (e0.GetPersistentMethodName(i) != ei.GetPersistentMethodName(i))
                    {
                        return false;
                    }
                    var t1 = e0.GetPersistentTarget(i) as Component;
                    var t2 = ei.GetPersistentTarget(i) as Component;
                    if (t1 != t2)
                    {
                        if (t1 != null ^ t2 != null)
                        {
                            return false;
                        }
                        if (t1 != null && (t1.GetType() != t2.GetType() || t1.name != t2.name || !MemberControl.IsHierarchyMatch(t1.transform, t2.transform)))
                        {
                            return false;
                        }
                    } else
                    {
                        // check arguments
                        var call0 = e0.GetPersistentCall(i);
                        var calli = ei.GetPersistentCall(i);
                        if (!UnityEventEx.IsArgumentEquals(call0, calli))
                        {
                            return false;
                        }

                    }
                }
                return true;
            }
            else
            {
                return base.MemberEquals(m, val0, vali);
            }
        }
    }

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

