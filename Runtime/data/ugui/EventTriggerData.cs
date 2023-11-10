//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static UnityEngine.EventSystems.EventTrigger;

    [Serializable]
    public class EventTriggerData : MonoData<EventTrigger>
    {
        [Store] public List<Entry> triggers;
        [HideInInspector] public bool triggers_mod;

        public override bool ValueEquals(object v0, object v1)
        {
            if (v0 is Entry e0)
            {
                var e1 = v1 as Entry;
                if (e0 != null ^ e1 != null)
                {
                    return false;
                } else if (e0 != e1)
                {
                    if (e0.eventID != e1.eventID)
                    {
                        return false;
                    } else
                    {
                        return base.ValueEquals(e0.callback, e1.callback);
                    }
                } else
                {
                    return true;
                }
            }
            return base.ValueEquals(v0, v1);
        }

        public override void ProcessMatchingRefs(object val, Transform rc, Transform r0)
        {
            if (val is Entry e)
            {
                e.callback.ReplaceMatchingTarget(rc, r0);
            } else
            {
                base.ProcessMatchingRefs(val, rc, r0);
            }
        }

    }
}

