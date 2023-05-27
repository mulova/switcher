//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------
namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    [Serializable, Preserve]
    public class ShadowData : MonoData<Shadow>
    {
        [Store, Preserve] public Color effectColor;
        [HideInInspector, Preserve] public bool effectColor_IsSet;
        [Store, Preserve] public Vector2 effectDistance;
        [HideInInspector, Preserve] public bool effectDistance_IsSet;
        [Store, Preserve] public bool useGraphicAlpha;
        [HideInInspector, Preserve] public bool useGraphicAlpha_IsSet;
    }
}

