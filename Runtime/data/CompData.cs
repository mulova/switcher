//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    [Serializable, UnityEngine.Scripting.Preserve]
    public abstract class CompData : ICompData
    {
        public const string IS_SET_SUFFIX = "_IsSet";
        public static readonly BindingFlags INSTANCE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        public static readonly BindingFlags FIELD_FLAG = INSTANCE_FLAGS & ~BindingFlags.SetProperty & ~BindingFlags.GetProperty;
        public static readonly BindingFlags PROPERTY_FLAG = INSTANCE_FLAGS & ~BindingFlags.GetField & ~BindingFlags.SetField;
        private static Dictionary<Type, List<MemberControl>> cache;

        public abstract Type srcType { get; }
        public abstract bool active { get; }
        public abstract Component target { get; set; }

        private static ILogger log => Debug.unityLogger;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            cache = null;
        }

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
            m.Collect(this, c);
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

        public virtual bool MemberEquals(MemberControl m, object val0, object vali)
        {
            if (val0 != null)
            {
                switch (val0)
                {
                    case float f0:
                        var f1 = (float)vali;
                        return Mathf.Abs(f1 - f0) <= Mathf.Epsilon;
                    case Vector2 v20:
                        var v21 = (Vector2)vali;
                        return v20.ApproximatelyEquals(v21);
                    case Vector3 v30:
                        var v31 = (Vector3)vali;
                        return v30.ApproximatelyEquals(v31);
                    case Vector4 v40:
                        var v41 = (Vector4)vali;
                        return v40.ApproximatelyEquals(v41);
                }
            }
            return val0.Equals(vali);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return target != null ? target.name : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if any of the CompData</returns>

        public List<MemberControl> ListChangedMembers()
        {
            var ret = new List<MemberControl>();
            var members = ListAttributedMembers();
            foreach (var m in members)
            {
                if (m.HasChanged(this))
                {
                    ret.Add(m);
                }
            }
            return ret;
        }

        protected virtual void SortMembers(List<MemberControl> members) { }

        public List<MemberControl> ListAttributedMembers()
        {
            return ListAttributedMembers(srcType, GetType(), SortMembers);
        }

        public MemberControl GetMember(string name)
        {
            return ListAttributedMembers().Find(m => m.name == name);
        }

        public static List<MemberControl> ListAttributedMembers(Type srcType, Type storeType, Action<List<MemberControl>> sorter)
        { 
            if (cache == null)
            {
                cache = new Dictionary<Type, List<MemberControl>>();
            }
            if (!cache.TryGetValue(srcType, out var list))
            {
                list = new List<MemberControl>();
                foreach (FieldInfo f in storeType.GetFields(FIELD_FLAG))
                {
                    var a = f.GetCustomAttribute<StoreAttribute>();
                    if (a != null)
                    {
                        var isSetField = storeType.GetField(f.Name + IS_SET_SUFFIX, FIELD_FLAG);
                        if (isSetField != null && isSetField.FieldType == typeof(bool))
                        {
                            var member = srcType.GetMember(f.Name, INSTANCE_FLAGS);
                            var slot = new MemberControl(f, isSetField, member.FirstOrDefault());
                            list.Add(slot);
                        } else
                        {
                            var member = srcType.GetMember(f.Name, INSTANCE_FLAGS);
                            var slot = new MemberControl(f, isSetField, member.FirstOrDefault());
                            list.Add(slot);
                            log?.LogFormat(LogType.Log, "{0}.{1}_IsSet field is missing", f.DeclaringType.Name, f.Name);
                        }
                    }
                }
                sorter?.Invoke(list);
                cache[srcType] = list;
            }
            return list;
        }
    }
}

