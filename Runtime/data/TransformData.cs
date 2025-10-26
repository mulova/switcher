//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using mulova.switcher.foundation;

namespace mulova.switcher
{
    using System;
    using UnityEngine;

    [Serializable]
    public class TransformData : CompData
    {
        [Store] public Vector3 localPosition;
        [HideInInspector] public bool localPosition_mod;
        [Store] public Quaternion localRotation;
        [HideInInspector] public bool localRotation_mod;
        [Store] public Vector3 localScale;
        [HideInInspector] public bool localScale_mod;
        [Store, Rename("Active")] public bool enabled;
        [HideInInspector] public bool enabled_mod;

        [SerializeField] protected Transform _target;

        public override bool active => enabled;
        public override Type srcType => typeof(Transform);

        public override Component target
        {
            get { return _target; }
            set { _target = value as Transform; }
        }

        public override void ApplyValue(MemberControl m, Component c, object val)
        {
            if (m.memberType == typeof(bool) && m.name == nameof(enabled))
            {
                c.gameObject.SetActive((bool)val);
            } else
            {
                base.ApplyValue(m, c, val);
            }
            (c as Transform).hasChanged = true;
        }

        public override object GetValueFrom(MemberControl m, Component c)
        {
            switch (m.name)
            {
                case nameof(enabled):
                    return c.gameObject.activeSelf;
                default:
                    return base.GetValueFrom(m, c);
            }
        }

        public override object GetValue(MemberControl m)
        {
            if (m.memberType == typeof(bool) && m.name == nameof(enabled))
            {
                return enabled;
            } else
            {
                return base.GetValue(m);
            }
        }

        public bool IsGameObjectActive()
        {
            return !enabled_mod || enabled;
        }

        public virtual bool TransformEquals(TransformData that)
        {
            return this.localPosition.ApproximatelyEquals(that.localPosition)
                && this.localRotation.ApproximatelyEquals(that.localRotation)
                && this.localScale.ApproximatelyEquals(that.localScale);
        }
    }
}

