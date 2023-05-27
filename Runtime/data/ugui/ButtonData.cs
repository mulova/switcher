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

    [Serializable, Preserve]
    public class ButtonData : SelectableData<Button>
    {
        [Store, Preserve] public Button.ButtonClickedEvent onClick;
        [HideInInspector, Preserve] public bool onClick_IsSet;

        protected override void CollectMember(MemberControl m, Component src, Transform rc, Transform r0)
        {
            if (r0 != rc && typeof(UnityEvent).IsAssignableFrom(m.memberType))
            {
                var vi = m.GetValue(src) as UnityEvent;
                if (vi != null)
                {
#if UNITY_EDITOR
                    //vi = vi.Clone();
                    for (int i = 0; i < vi.GetPersistentEventCount(); ++i)
                    {
                        var t = vi.GetPersistentTarget(i) as MonoBehaviour;
                        if (t != null && t.transform.IsChildOf(rc))
                        {
                            var methodName = "";
                            try
                            {
                                Transform trans0 = MemberControl.GetMatchingTransform(t.transform, rc, r0);
                                var t0 = trans0.GetComponent(t.GetType());
                                methodName = vi.GetPersistentMethodName(i);
                                var methodInfo = t0.GetType().GetMethod(methodName);
                                var paramArr = methodInfo.GetParameters();
                                if (paramArr == null || paramArr.Length == 0)
                                {
                                    var action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), t0, methodName);
                                    UnityEditor.Events.UnityEventTools.RegisterPersistentListener(vi, i, action);
                                } else
                                {
                                    Debug.LogWarning($"Delegate call {t.GetType().Name}.{methodName}({t.name}) is ignored. Call with parameter is not supported");
                                }
                            } catch (Exception ex)
                            {
                                Debug.LogErrorFormat(src, "{0}.{1}\n{2}", t.GetType(), methodName, ex);
                                throw;
                            }
                        }
                    }
#endif
                    m.SetValue(this, vi);
                } else
                {
                    m.SetValue(this, null);
                }
            } else
            {
                base.CollectMember(m, src, rc, r0);
            }
        }

        public override bool MemberEquals(MemberControl m, object val0, object vali)
        {
            if (typeof(UnityEvent).IsAssignableFrom(m.memberType))
            {
                var v0 = val0 as UnityEvent;
                var vi = vali as UnityEvent;
                if (vi == null)
                {
                    return false;
                }
                if (v0.GetPersistentEventCount() == vi.GetPersistentEventCount())
                {
                    for (int i = 0; i < onClick.GetPersistentEventCount(); ++i)
                    {
                        if (v0.GetPersistentMethodName(i) != vi.GetPersistentMethodName(i))
                        {
                            return false;
                        }
                        var t1 = v0.GetPersistentTarget(i) as MonoBehaviour;
                        var t2 = vi.GetPersistentTarget(i) as MonoBehaviour;
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
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.MemberEquals(m, val0, vali);
            }
        }
    }

}

