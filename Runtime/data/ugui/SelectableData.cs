//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;
    using static UnityEngine.UI.Selectable;

    [Serializable, Preserve]
    public abstract class SelectableData<S> : MonoData<S> where S : Selectable
    {
        [Store, Preserve] public bool interactable;
        [HideInInspector, Preserve] public bool interactable_IsSet;
        [Store, Preserve] public Transition transition;
        [HideInInspector, Preserve] public bool transition_IsSet;
        [Store, Preserve] public Navigation navigation;
        [HideInInspector, Preserve] public bool navigation_IsSet;
        [Store, Preserve] public Graphic targetGraphic;
        [HideInInspector, Preserve] public bool targetGraphic_IsSet;
        [Store, Preserve] public ColorBlock colors;
        [HideInInspector, Preserve] public bool colors_IsSet;
        [Store, Preserve] public SpriteState spriteState;
        [HideInInspector, Preserve] public bool spriteState_IsSet;
        [Store, Preserve] public AnimationTriggers animationTriggers;
        [HideInInspector, Preserve] public bool animationTriggers_IsSet;

        public override bool MemberEquals(MemberControl m, object val1, object val2)
        {
            if (m.memberType == typeof(AnimationTriggers))
            {
                return TriggerEquals(val1 as AnimationTriggers, val2 as AnimationTriggers);
            } else
            {
                return base.MemberEquals(m, val1, val2);
            }
        }

        private bool TriggerEquals(AnimationTriggers t1, AnimationTriggers t2)
        {
            if (t1 == t2)
            {
                return true;
            }
            if (t1 == null || t2 == null)
            {
                return false;
            }
            return t1.disabledTrigger == t2.disabledTrigger
                && t1.highlightedTrigger == t2.highlightedTrigger
                && t1.normalTrigger == t2.normalTrigger
                && t1.pressedTrigger == t2.pressedTrigger
                && t1.selectedTrigger == t2.selectedTrigger;
        }
    }
}

