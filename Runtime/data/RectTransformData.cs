//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System.Reflection;

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

        protected override IReadOnlyList<string> memberOrder => new[] { nameof(enabled), nameof(pivot), nameof(sizeDelta), nameof(anchorMin), nameof(anchorMax), nameof(anchoredPosition) };

        public override Type srcType => typeof(RectTransform);

        public override void SetValue(MemberControl m, Component c, object val)
        {
            if (m.memberType == typeof(Vector3) && m.name == nameof(localPosition))
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

        protected override bool IsCollectable(MemberControl m)
        {
            var drivenField = typeof(RectTransform).GetProperty("drivenProperties", MemberControl.PROPERTY_FLAG);
            var drivenProperties = (DrivenTransformProperties)drivenField.GetValue(target);
            switch (m.name)
            {
                case nameof(localPosition):
                    return false;
                case nameof(anchorMax):
                    return !IsFullyDriven(drivenProperties, DrivenTransformProperties.AnchorMax);
                case nameof(anchorMin):
                    return !IsFullyDriven(drivenProperties, DrivenTransformProperties.AnchorMin);
                case nameof(sizeDelta):
                    return !IsFullyDriven(drivenProperties, DrivenTransformProperties.SizeDelta);
                case nameof(pivot):
                    return !IsFullyDriven(drivenProperties, DrivenTransformProperties.Pivot);
                case nameof(localScale):
                    return !IsFullyDriven(drivenProperties, DrivenTransformProperties.Scale);
                case nameof(anchoredPosition):
                    return !IsFullyDriven(drivenProperties, DrivenTransformProperties.AnchoredPosition);
                default:
                    return true;
            }

            // static bool IsFree(DrivenTransformProperties prop, DrivenTransformProperties flag) => (prop & flag) == 0;
            static bool IsFullyDriven(DrivenTransformProperties prop, DrivenTransformProperties flag)
            {
                var driven = prop & flag;
                return driven == flag;
            }
        }
    }
}

