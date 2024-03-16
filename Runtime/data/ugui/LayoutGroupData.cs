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
    }
}

