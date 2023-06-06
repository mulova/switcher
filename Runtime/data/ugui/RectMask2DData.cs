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
    public class RectMask2DData : MonoData<RectMask2D>
    {
        [Store] public Vector4 padding;
        [HideInInspector] public bool padding_IsSet;
        [Store] public Vector2Int softness;
        [HideInInspector] public bool softness_IsSet;
    }
}

