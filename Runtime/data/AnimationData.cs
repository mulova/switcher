//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;

    [Serializable]
    public class AnimationData : MonoData<Animation>
    {
        [Store] public AnimationClip clip;
        [HideInInspector] public bool clip_mod;
        [Store] public bool playAutomatically;
        [HideInInspector] public bool playAutomatically_mod;
        [Store] public WrapMode wrapMode;
        [HideInInspector] public bool wrapMode_mod;

        public override void ApplyTo(Component c)
        {
            base.ApplyTo(c);
            if (Application.isPlaying)
            {
                _target.Play(clip.name);
            }
        }
    }
}

