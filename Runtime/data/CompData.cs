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
    using IList = System.Collections.IList;

    [Serializable]
    public abstract class CompData
    {
        public abstract Type srcType { get; }
        public abstract bool active { get; }
        public abstract Component target { get; set; }
        [HideInInspector] public bool isActiveAndEnabled = true;
        /// <summary>
        /// if srcType is the same, higher priority CompData is used.
        /// </summary>
        public virtual int priority { get; } = 0;

        public void ApplyTo(Component c)
        {
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (m.HasChanged(this))
                {
                    SetValue(m, c);
                }
            }
            OnApplyEnd();
        }
        
        protected virtual void OnApplyEnd() {}

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

        public virtual void SetValue(MemberControl m, Component c, object value)
        {
            if (isActiveAndEnabled)
            {
                m.Apply(c, value);  
            }
        } 

        public void SetValue(MemberControl m, Component c)
        {
            SetValue(m, c, GetValue(m));
        }

        public void Collect(Component src, bool changedOnly)
        {
            target = src;
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (!changedOnly || m.HasChanged(this))
                {
                    if (IsCollectable(m))
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

            isActiveAndEnabled = src switch
            {
                Behaviour b => b.isActiveAndEnabled,
                _ => src.gameObject.activeInHierarchy
            };
        }

        protected virtual bool IsCollectable(MemberControl m) => true;

        public virtual void Postprocess(IReadOnlyList<CompData> list) { }

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

        public bool MemberEquals(MemberControl m, object v0, object v1)
        {
            if (v0 != null)
            {
                if (typeof(IList).IsAssignableFrom(v0.GetType()))
                {
                    var l0 = v0 as IList;
                    var l1 = v1 as IList;
                    if (l0 != l1)
                    {
                        if (l0 == null || l1 == null)
                        {
                            return false;
                        }
                        if (l0.Count != l1.Count)
                        {
                            return false;
                        }
                        for (int i=0; i<l0.Count; ++i)
                        {
                            if (!ValueEquals(l0[i], l1[i]))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                } else
                {
                    return ValueEquals(v0, v1);
                }
            }
            else
            {
                return v1 == null;
            }
        }

        public virtual bool ValueEquals(object v0, object v1)
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
                case RectOffset r:
                {
                    var pad0 = r;
                    var pad1 = v1 as RectOffset;
                    return pad0.top == pad1.top
                           && pad0.bottom == pad1.bottom
                           && pad0.left == pad1.left
                           && pad0.right == pad1.right
                           && pad0.horizontal == pad1.horizontal
                           && pad0.vertical == pad1.vertical;
                }
                default:
                    return v0.Equals(v1);
            }
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

        public List<MemberControl> ListUnchangedMembers() => ListAttributedMembers().FindAll(m => !m.HasChanged(this));

        /// <summary>
        /// Use when the value setting order is important like RectTransform
        /// </summary>
        /// <param name="members"></param>
        protected virtual IReadOnlyList<string> memberOrder { get; }

        public List<MemberControl> ListAttributedMembers()
        {
            return MemberControl.ListAttributedMembers(srcType, GetType(), memberOrder);
        }

        public MemberControl GetMember(string name)
        {
            return ListAttributedMembers().Find(m => m.name == name);
        }

        public void ReplaceRefs(Component comp, Component comp0, Transform rc, Transform r0)
        {
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                try
                {
                    var val = GetValueFrom(m, comp);
                    var val0 = GetValueFrom(m, comp0);
                    if (val != null && rc != r0)
                    {
                        if (val is IList list)
                        {
                            ProcessMatchingList(list, val0 as IList, rc, r0);
                        }
                        else
                        {
                            if (val is Component c)
                            {
                                var match = c.GetHierarchyPair(rc, r0);
                                if (match != null)
                                {
                                    SetValue(m, comp, match);
                                }
                            }
                            else if (val is GameObject o)
                            {
                                var t = o.transform;
                                var match = t.GetHierarchyPair(rc, r0);
                                if (match != null)
                                {
                                    SetValue(m, comp, match.gameObject);
                                }
                            } else
                            {
                                ProcessMatchingRefs(val, val0, rc, r0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Fail to replace reference: {0} {1}.{2}\n{3}", comp.transform.GetScenePath(), m.storeField.ReflectedType.FullName, m.storeField.Name, ex);
                }
            }
        }

        protected virtual void ProcessMatchingList(IList list, IList list0, Transform rc, Transform r0)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                ProcessMatchingRefs(list[i], list0[i] ,rc, r0);
            }
        }

        protected virtual void ProcessMatchingRefs(object val, object val0, Transform rc, Transform r0)
        {
            if (typeof(UnityEventBase).IsAssignableFrom(val.GetType()))
            {
                var e = val as UnityEventBase;
                e.ReplaceMatchingTarget(rc, r0);
            }
        }

        public virtual bool IsValid()
        {
            return target != null;
        }
    }
}

