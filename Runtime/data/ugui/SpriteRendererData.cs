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
        [HideInInspector] public bool sprite_mod;
        [Store] public SpriteDrawMode drawMode;
        [HideInInspector] public bool drawMode_mod;
        [Store] public Vector2 size;
        [HideInInspector] public bool size_mod;
        [Store] public float adaptiveModeThreshold;
        [HideInInspector] public bool adaptiveModeThreshold_mod;
        [Store] public SpriteTileMode tileMode;
        [HideInInspector] public bool tileMode_mod;
        [Store] public Color color;
        [HideInInspector] public bool color_mod;
        [Store] public SpriteMaskInteraction maskInteraction;
        [HideInInspector] public bool maskInteraction_mod;
        [Store] public bool flipX;
        [HideInInspector] public bool flipX_mod;
        [Store] public bool flipY;
        [HideInInspector] public bool flipY_mod;
        [Store] public SpriteSortPoint spriteSortPoint;
        [HideInInspector] public bool spriteSortPoint_mod;
    }
}

