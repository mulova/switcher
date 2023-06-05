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
    public class ImageData : GraphicData<Image>
    {
        [Store] public Sprite sprite;
        [HideInInspector] public bool sprite_IsSet;
        [Store] public Image.Type type;
        [HideInInspector] public bool type_IsSet;
        [Store] public float fillAmount;
        [HideInInspector] public bool fillAmount_IsSet;
        [Store] public bool fillCenter;
        [HideInInspector] public bool fillCenter_IsSet;
        [Store] public bool fillClockwise;
        [HideInInspector] public bool fillClockwise_IsSet;
        [Store] public Image.FillMethod fillMethod;
        [HideInInspector] public bool fillMethod_IsSet;
        [Store] public int fillOrigin;
        [HideInInspector] public bool fillOrigin_IsSet;
        [Store] public bool useSpriteMesh;
        [HideInInspector] public bool useSpriteMesh_IsSet;
        [Store] public bool preserveAspect;
        [HideInInspector] public bool preserveAspect_IsSet;
        [Store] public bool maskable;
        [HideInInspector] public bool maskable_IsSet;
    }
}

