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
    public class TransformData : CompData
    {
        [Store] public Vector3 localPosition;
        [HideInInspector] public bool localPosition_mod;
        [Store] public Quaternion localRotation;
        [HideInInspector] public bool localRotation_mod;
        [Store] public Vector3 localScale;
        [HideInInspector] public bool localScale_mod;
        [Store] public bool enabled;
        [HideInInspector] public bool enabled_mod;

        [UnityEngine.Serialization.FormerlySerializedAs("trans")]
        [SerializeField] protected Transform _target;

        public override bool active => enabled;
        public override Type srcType => typeof(Transform);

        public override Component target
        {
            get { return _target; }
            set { _target = value as Transform; }
        }

        protected override void ApplyMember(MemberControl m, Component c)
        {
            if (m.memberType == typeof(bool) && m.name == nameof(enabled))
            {
                c.gameObject.SetActive(enabled);
            } else
            {
                base.ApplyMember(m, c);
            }
            (c as Transform).hasChanged = true;
        }

        protected override void CollectMember(MemberControl m, Component c, Transform rc, Transform r0)
        {
            if (m.name == nameof(enabled))
            {
                enabled = c.gameObject.activeSelf;
            } else
            {
                base.CollectMember(m, c, rc, r0);
            }
        }

        public virtual bool TransformEquals(TransformData that)
        {
            return this.localPosition.ApproximatelyEquals(that.localPosition)
                && this.localRotation.ApproximatelyEquals(that.localRotation)
                && this.localScale.ApproximatelyEquals(that.localScale);
        }

        public override string ToString()
        {
            return target != null ? target.name : null;
        }
    }
}

