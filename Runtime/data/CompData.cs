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
    using UnityEngine.Events;

    [Serializable, UnityEngine.Scripting.Preserve]
    public abstract class CompData : ICompData
    {
        public abstract Type srcType { get; }
        public abstract bool active { get; }
        public abstract Component target { get; set; }

        public virtual void ApplyTo(Component c)
        {
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (m.HasChanged(this))
                {
                    ApplyMember(m, c);
                }
            }
        }

        protected virtual void ApplyMember(MemberControl m, Component target)
        {
            m.Apply(target, this);
        }

        public virtual void Collect(Component src, Transform rc, Transform r0, bool changedOnly)
        {
            target = src;
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (!changedOnly || m.HasChanged(this))
                {
                    CollectMember(m, src, rc, r0);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="c">the component to collect data</param>
        /// <param name="r0">the first root</param>
        /// <param name="ri">the i th root</param>
        protected virtual void CollectMember(MemberControl m, Component c, Transform rc, Transform r0)
        {
            m.Collect(this, c, rc, r0);
        }

        public override bool Equals(object obj)
        {
            var that = obj as CompData;

            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                var val1 = m.GetValue(this);
                var val2 = m.GetValue(that);
                if (val1 == null ^ val2 == null)
                {
                    return false;
                }
                if (val1 != val2 && val1 != null && !MemberEquals(m, val1, val2))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool MemberEquals(MemberControl m, object v0, object v1)
        {
            if (v0 != null)
            {
                switch (v0)
                {
                    case float f0:
                        var f1 = (float)v1;
                        return Mathf.Abs(f1 - f0) <= Mathf.Epsilon;
                    case Vector2 v20:
                        var v21 = (Vector2)v1;
                        return v20.ApproximatelyEquals(v21);
                    case Vector3 v30:
                        var v31 = (Vector3)v1;
                        return v30.ApproximatelyEquals(v31);
                    case Vector4 v40:
                        var v41 = (Vector4)v1;
                        return v40.ApproximatelyEquals(v41);
                    case UnityEventBase u0:
                        {
                            var u1 = v1 as UnityEventBase;
                            return u0.PersistentEventEquals(u1);
                        }
                    default:
                        return v0.Equals(v1);
                }
            }
            return v0 == v1;
        }

        public override int GetHashCode() => target != null ? target.GetHashCode() : base.GetHashCode();

        public override string ToString() => target != null ? target.ToString() : null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if any of the CompData</returns>
        public List<MemberControl> ListChangedMembers() => ListAttributedMembers().FindAll(m => m.HasChanged(this));

        /// <summary>
        /// Use when the value setting order is important like RectTransform
        /// </summary>
        /// <param name="members"></param>
        protected virtual void SortMembers(List<MemberControl> members) { }

        public List<MemberControl> ListAttributedMembers()
        {
            return MemberControl.ListAttributedMembers(srcType, GetType(), SortMembers);
        }

        public MemberControl GetMember(string name)
        {
            return ListAttributedMembers().Find(m => m.name == name);
        }
    }
}

