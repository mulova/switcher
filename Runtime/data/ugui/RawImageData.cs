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
    public class RawImageData : MaskableGraphicData<RawImage>
    {
        [Store] public Texture texture;
        [HideInInspector] public bool texture_mod;
        [Store] public Rect uvRect;
        [HideInInspector] public bool uvRect_mod;
    }
}

