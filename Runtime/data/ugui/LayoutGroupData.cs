//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [Serializable]
    public class LayoutGroupData : MonoData<LayoutGroup>
    {
        [Store] public TextAnchor childAlignment;
        [HideInInspector] public bool childAlignment_mod;
        [Store] public RectOffset padding;
        [HideInInspector] public bool padding_mod;

        public override bool MemberEquals(MemberControl m, object v0, object v1)
        {
            if (m.name == nameof(padding))
            {
                var pad0 = v0 as RectOffset;
                var pad1 = v1 as RectOffset;
                return pad0.top == pad1.top
                    && pad0.bottom == pad1.bottom
                    && pad0.left == pad1.left
                    && pad0.right == pad1.right
                    && pad0.horizontal == pad1.horizontal
                    && pad0.vertical == pad1.vertical;
            } else
            {
                return base.MemberEquals(m, v0, v1);
            }
        }
    }
}

