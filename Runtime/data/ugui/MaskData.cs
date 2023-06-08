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
    public class MaskData : MonoData<Mask>
    {
        [Store] public bool showMaskGraphic;
        [HideInInspector] public bool showMaskGraphic_mod;
    }
}

