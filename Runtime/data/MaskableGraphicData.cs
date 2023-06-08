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
    public abstract class MaskableGraphicData<G> : GraphicData<G> where G : MaskableGraphic
    {
        [Store] public bool isMaskingGraphic;
        [HideInInspector] public bool isMaskingGraphic_mod;
        [Store] public bool maskable;
        [HideInInspector] public bool maskable_mod;
    }
}

