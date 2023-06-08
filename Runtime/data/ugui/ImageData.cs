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
    using static UnityEngine.UI.ContentSizeFitter;

    [Serializable]
    public class ImageData : MaskableGraphicData<Image>
    {
        [Store] public Sprite sprite;
        [HideInInspector] public bool sprite_mod;
        [Store] public Image.Type type;
        [HideInInspector] public bool type_mod;
        [Store] public float fillAmount;
        [HideInInspector] public bool fillAmount_mod;
        [Store] public bool fillCenter;
        [HideInInspector] public bool fillCenter_mod;
        [Store] public bool fillClockwise;
        [HideInInspector] public bool fillClockwise_mod;
        [Store] public Image.FillMethod fillMethod;
        [HideInInspector] public bool fillMethod_mod;
        [Store] public int fillOrigin;
        [HideInInspector] public bool fillOrigin_mod;
        [Store] public bool useSpriteMesh;
        [HideInInspector] public bool useSpriteMesh_mod;
        [Store] public bool preserveAspect;
        [HideInInspector] public bool preserveAspect_mod;
    }
}

