//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using static UnityEngine.UI.Selectable;

    [Serializable]
    public abstract class SelectableData<S> : MonoData<S> where S : Selectable
    {
        [Store] public bool interactable;
        [HideInInspector] public bool interactable_IsSet;
        [Store] public Transition transition;
        [HideInInspector] public bool transition_IsSet;
        [Store] public Navigation navigation;
        [HideInInspector] public bool navigation_IsSet;
        [Store] public Graphic targetGraphic;
        [HideInInspector] public bool targetGraphic_IsSet;
        [Store] public ColorBlock colors;
        [HideInInspector] public bool colors_IsSet;
        [Store] public SpriteState spriteState;
        [HideInInspector] public bool spriteState_IsSet;
        [Store] public AnimationTriggers animationTriggers;
        [HideInInspector] public bool animationTriggers_IsSet;

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

