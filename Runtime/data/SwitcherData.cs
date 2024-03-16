//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class SwitcherData : MonoData<Switcher>
    {
        [Store] public List<Case> cases;
        [HideInInspector] public bool cases_mod;

        protected override void ProcessMatchingRefs(object val, object val0, Transform rc, Transform r0)
        {
            if (val is Case that)
            {
                if (cases[0].data.Count != that.data.Count)
                {
                    Debug.LogError($"Case data count mismatch between {rc.name} and {r0.name}");
                }

                for (var id = 0; id < cases[0].data.Count; ++id)
                {
                    if (cases[0].data[id].srcType != that.data[id].srcType)
                    {
                        Debug.LogError($"data type mismatch between {rc.name}({id}) and {r0.name}({id})");
                        break;
                    }

                    var case0 = val0 as Case;
                    that.data[id].target = case0.data[id].target;
                }
            }
            else
            {
                base.ProcessMatchingRefs(val, val0, rc, r0);
            }

        }

        protected override void OnApplyEnd()
        {
            targetCasted.Reset();
        }

        public override bool ValueEquals(object v0, object v1) => v0 switch
        {
            Case c0 => CaseEquals(c0, v1 as Case),
            _ => base.ValueEquals(v0, v1)
        };

        public static bool CaseEquals(Case c0, Case c1)
        {
            if (c0.name != c1.name)
            {
                return false;
            }
            return true;
        }
    }
}
