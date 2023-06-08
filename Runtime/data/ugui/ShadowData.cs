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
    public class ShadowData : MonoData<Shadow>
    {
        [Store] public Color effectColor;
        [HideInInspector] public bool effectColor_mod;
        [Store] public Vector2 effectDistance;
        [HideInInspector] public bool effectDistance_mod;
        [Store] public bool useGraphicAlpha;
        [HideInInspector] public bool useGraphicAlpha_mod;
    }
}

