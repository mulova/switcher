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

    [Serializable, Preserve]
    public class TransformData : CompData
    {
        [Store, Preserve] public Vector3 localPosition;
        [HideInInspector, Preserve] public bool localPosition_IsSet;
        [Store, Preserve] public Quaternion localRotation;
        [HideInInspector, Preserve] public bool localRotation_IsSet;
        [Store, Preserve] public Vector3 localScale;
        [HideInInspector, Preserve] public bool localScale_IsSet;
        [Store, Preserve] public bool enabled;
        [HideInInspector, Preserve] public bool enabled_IsSet;

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

