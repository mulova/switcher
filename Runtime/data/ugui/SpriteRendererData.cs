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

    [Serializable, Preserve]
    public class SpriteRendererData : MonoData<SpriteRenderer>
    {
        [Store, Preserve] public Sprite sprite;
        [HideInInspector, Preserve] public bool sprite_IsSet;
    }
}

