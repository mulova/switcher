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
    public abstract class GraphicData<G> : MonoData<G> where G : Graphic
    {
        [Store, Preserve] public bool raycastTarget;
        [HideInInspector, Preserve] public bool raycastTarget_IsSet;
        [Store, Preserve] public Color color;
        [HideInInspector, Preserve] public bool color_IsSet;
    }
}

