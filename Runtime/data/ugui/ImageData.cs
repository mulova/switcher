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
    public class ImageData : GraphicData<Image>
    {
        [Store, Preserve] public Sprite sprite;
        [HideInInspector, Preserve] public bool sprite_IsSet;
        [Store, Preserve] public Image.Type type;
        [HideInInspector, Preserve] public bool type_IsSet;
        [Store, Preserve] public float fillAmount;
        [HideInInspector, Preserve] public bool fillAmount_IsSet;
        [Store, Preserve] public bool fillCenter;
        [HideInInspector, Preserve] public bool fillCenter_IsSet;
        [Store, Preserve] public bool fillClockwise;
        [HideInInspector, Preserve] public bool fillClockwise_IsSet;
        [Store, Preserve] public Image.FillMethod fillMethod;
        [HideInInspector, Preserve] public bool fillMethod_IsSet;
        [Store, Preserve] public int fillOrigin;
        [HideInInspector, Preserve] public bool fillOrigin_IsSet;
        [Store, Preserve] public bool useSpriteMesh;
        [HideInInspector, Preserve] public bool useSpriteMesh_IsSet;
        [Store, Preserve] public bool preserveAspect;
        [HideInInspector, Preserve] public bool preserveAspect_IsSet;
        [Store, Preserve] public bool maskable;
        [HideInInspector, Preserve] public bool maskable_IsSet;
    }
}

