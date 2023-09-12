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
    public abstract class MonoData<T> : CompData where T: Component
    {
        [Store] public bool enabled;
        [HideInInspector] public bool enabled_mod;

        public override bool active => enabled;
        public override Type srcType => typeof(T);

        [SerializeField] protected T _target;
        public override Component target
        {
            get { return _target; }
            set { _target = value as T; }
        }
    }
}

