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
    public class LayoutElementData : MonoData<LayoutElement>
    {
        [Store] public float preferredWidth;
        [HideInInspector] public bool preferredWidth_mod;
        [Store] public float preferredHeight;
        [HideInInspector] public bool preferredHeight_mod;
        [Store] public float flexibleWidth;
        [HideInInspector] public bool flexibleWidth_mod;
        [Store] public float flexibleHeight;
        [HideInInspector] public bool flexibleHeight_mod;
        [Store] public int layoutPriority;
        [HideInInspector] public bool layoutPriority_mod;
        [Store] public float minWidth;
        [HideInInspector] public bool minWidth_mod;
        [Store] public float minHeight;
        [HideInInspector] public bool minHeight_mod;
        [Store] public bool ignoreLayout;
        [HideInInspector] public bool ignoreLayout_mod;
    }
}
