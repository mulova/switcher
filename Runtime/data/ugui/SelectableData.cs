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
        [HideInInspector] public bool interactable_mod;
        [Store] public Transition transition;
        [HideInInspector] public bool transition_mod;
        [Store] public Navigation navigation;
        [HideInInspector] public bool navigation_mod;
        [Store] public Graphic targetGraphic;
        [HideInInspector] public bool targetGraphic_mod;
        [Store] public ColorBlock colors;
        [HideInInspector] public bool colors_mod;
        [Store] public SpriteState spriteState;
        [HideInInspector] public bool spriteState_mod;
        [Store] public AnimationTriggers animationTriggers;
        [HideInInspector] public bool animationTriggers_mod;

        public override bool ValueEquals(object val1, object val2) => val1 switch
        {
            AnimationTriggers t1 => TriggerEquals(t1, val2 as AnimationTriggers),
            _ => base.ValueEquals(val1, val2)
        };

        public static bool TriggerEquals(AnimationTriggers t1, AnimationTriggers t2)
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

