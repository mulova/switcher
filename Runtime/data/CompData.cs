﻿//----------------------------------------------
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

    [Serializable]
    public abstract class CompData
    {
        public abstract Type srcType { get; }
        public abstract bool active { get; }
        public abstract Component target { get; set; }
        /// <summary>
        /// if srcType is the same, higher priority CompData is used.
        /// </summary>
        public virtual int priority { get; } = 0;

        public virtual void ApplyTo(Component c)
        {
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (m.HasChanged(this))
                {
                    SetValue(m, c);
                }
            }
        }

        public bool IsApplied()
        {
            if (target == null)
            {
                return false;
            }
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (m.HasChanged(this))
                {
                    var v1 = GetValueFrom(m, target);
                    var v2 = GetValue(m);
                    if (!MemberEquals(m, v1, v2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual object GetValue(MemberControl m)
        {
            try
            {
                return m.storeField.GetValue(this);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("{0}.{1}\n{2}", m.memberType.FullName, m.name, ex);
                throw;
            }
        }

        public virtual object GetValueFrom(MemberControl m, Component c) => m.GetValue(c);

        public virtual void SetValue(MemberControl m, Component c, object value) => m.Apply(c, value);

        public void SetValue(MemberControl m, Component c) => SetValue(m, c, GetValue(m));

        public virtual void Collect(Component src, bool changedOnly)
        {
            target = src;
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (!changedOnly || m.HasChanged(this))
                {
                    if (IsCollectable(m, src))
                    {
                        try
                        {
                            var val = GetValueFrom(m, src);
                            m.StoreValue(this, val);
                        }
                        catch
                        {
                            Debug.LogErrorFormat("{0}.{1}", m.storeField.ReflectedType.FullName, m.storeField.Name);
                            throw;
                        }
                    }
                }
            }
        }

        protected virtual bool IsCollectable(MemberControl m, Component c) => true;

        public override bool Equals(object obj)
        {
            var that = obj as CompData;

            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                var val1 = this.GetValue(m);
                var val2 = that.GetValue(m);
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

        public override string ToString()
        {
            return target != null ? $"{GetType().Name} - {target.name} ({srcType.Name})" : GetType().Name;
        }

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

        public void ReplaceRefs(Component comp, Transform rc, Transform r0)
        {
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                try
                {
                    var val = GetValueFrom(m, comp);
                    if (val != null && rc != r0)
                    {
                        if (m.IsTypeOf(typeof(UnityEventBase)))
                        {
                            var e = val as UnityEventBase;
                            e.ReplaceMatchingTarget(rc, r0);
                        }
                        else if (m.IsTypeOf(typeof(Component)))
                        {
                            var c = val as Component;
                            var match = c.GetHierarchyPair(rc, r0);
                            if (match != null)
                            {
                                val = match;
                            }
                        }
                        else if (m.IsTypeOf(typeof(GameObject)))
                        {
                            var o = val as GameObject;
                            var t = o.transform;
                            var match = t.GetHierarchyPair(rc, r0);
                            if (match != null)
                            {
                                val = match.gameObject;
                            }
                        }
                    }

                    SetValue(m, comp, val);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Fail to replace reference: {0} {1}.{2}\n{3}", comp.transform.GetScenePath(), m.storeField.ReflectedType.FullName, m.storeField.Name, ex);
                }
            }
        }
    }
}

