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
    public abstract class HorizontalOrVerticalLayoutGroupData : LayoutGroupData
    {
        [Store] public float spacing;
        [HideInInspector] public bool spacing_mod;
        [Store] public bool childForceExpandWidth;
        [HideInInspector] public bool childForceExpandWidth_mod;
        [Store] public bool childForceExpandHeight;
        [HideInInspector] public bool childForceExpandHeight_mod;
        [Store] public bool childControlWidth;
        [HideInInspector] public bool childControlWidth_mod;
        [Store] public bool childControlHeight;
        [HideInInspector] public bool childControlHeight_mod;
        [Store] public bool childScaleWidth;
        [HideInInspector] public bool childScaleWidth_mod;
        [Store] public bool childScaleHeight;
        [HideInInspector] public bool childScaleHeight_mod;
        [Store] public bool reverseArrangement;
        [HideInInspector] public bool reverseArrangement_mod;
    }
}

