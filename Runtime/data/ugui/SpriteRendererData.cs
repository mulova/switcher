//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SpriteRendererData : MonoData<SpriteRenderer>
    {
        [Store] public Sprite sprite;
        [HideInInspector] public bool sprite_IsSet;
        [Store] public SpriteDrawMode drawMode;
        [HideInInspector] public bool drawMode_IsSet;
        [Store] public Vector2 size;
        [HideInInspector] public bool size_IsSet;
        [Store] public float adaptiveModeThreshold;
        [HideInInspector] public bool adaptiveModeThreshold_IsSet;
        [Store] public SpriteTileMode tileMode;
        [HideInInspector] public bool tileMode_IsSet;
        [Store] public Color color;
        [HideInInspector] public bool color_IsSet;
        [Store] public SpriteMaskInteraction maskInteraction;
        [HideInInspector] public bool maskInteraction_IsSet;
        [Store] public bool flipX;
        [HideInInspector] public bool flipX_IsSet;
        [Store] public bool flipY;
        [HideInInspector] public bool flipY_IsSet;
        [Store] public SpriteSortPoint spriteSortPoint;
        [HideInInspector] public bool spriteSortPoint_IsSet;
    }
}

