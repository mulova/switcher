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

    [Serializable]
    public class RectTransformData : TransformData
    {
        public bool ignoreAnchoredPosition;
        [Store] public Vector2 anchoredPosition;
        [HideInInspector] public bool anchoredPosition_mod;
        [Store] public Vector2 anchorMax;
        [HideInInspector] public bool anchorMax_mod;
        [Store] public Vector2 anchorMin;
        [HideInInspector] public bool anchorMin_mod;
        [Store] public Vector2 pivot;
        [HideInInspector] public bool pivot_mod;
        [Store] public Vector2 sizeDelta;
        [HideInInspector] public bool sizeDelta_mod;

        public override Type srcType => typeof(RectTransform);

        public override void SetValue(MemberControl m, Component c, object val)
        {
            if (m.memberType == typeof(bool) && m.name == nameof(enabled))
            {
                c.gameObject.SetActive((bool)val);
            }
            else if (m.memberType == typeof(Vector3) && m.name == nameof(localPosition))
            {
                // skip
            } else if (m.memberType == typeof(Vector2) && m.name == nameof(anchoredPosition))
            {
                if (!ignoreAnchoredPosition)
                {
                    m.Apply(c, this);
                    (c as RectTransform).hasChanged = true;
                }
            }
            else
            {
                base.SetValue(m, c, val);
                (c as RectTransform).hasChanged = true;
            }
        }

        protected override void SortMembers(List<MemberControl> members)
        {
            var order = new[] { nameof(enabled), nameof(pivot), nameof(sizeDelta), nameof(anchorMin), nameof(anchorMax), nameof(anchoredPosition) };
            members.Sort((a, b) =>
            {
                foreach (var o in order)
                {
                    if (a.name == o)
                    {
                        return -1;
                    } else if (b.name == o)
                    {
                        return 1;
                    }
                }
                return a.name.CompareTo(b.name);
            });
        }

        protected override bool IsCollectable(MemberControl m, Component c)
        {
            switch (m.name)
            {
                case nameof(localPosition):
                    return false;
                default:
                    return true;
            }
        }
    }
}

