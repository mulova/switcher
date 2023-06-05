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
    public abstract class GraphicData<G> : MonoData<G> where G : Graphic
    {
        [Store] public bool raycastTarget;
        [HideInInspector] public bool raycastTarget_IsSet;
        [Store] public Color color;
        [HideInInspector] public bool color_IsSet;
        [Store] public Material material;
        [HideInInspector] public bool material_IsSet;
        [Store] public Texture mainTexture;
        [HideInInspector] public bool mainTexture_IsSet;
        [Store] public Vector4 raycastPadding;
        [HideInInspector] public bool raycastPadding_IsSet;
    }
}

