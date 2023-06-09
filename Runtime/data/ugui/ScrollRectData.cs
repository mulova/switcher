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
    using static UnityEngine.UI.ScrollRect;

    [Serializable]
    public class ScrollRectData : MonoData<ScrollRect>
    {
        [Store] public RectTransform content;
        [HideInInspector] public bool content_mod;
        [Store] public float decelerationRate;
        [HideInInspector] public bool decelerationRate_mod;
        [Store] public float elasticity;
        [HideInInspector] public bool elasticity_mod;
        [Store] public bool horizontal;
        [HideInInspector] public bool horizontal_mod;
        [Store] public Scrollbar horizontalScrollbar;
        [HideInInspector] public bool horizontalScrollbar_mod;
        [Store] public float horizontalScrollbarSpacing;
        [HideInInspector] public bool horizontalScrollbarSpacing_mod;
        [Store] public ScrollbarVisibility horizontalScrollbarVisibility;
        [HideInInspector] public bool horizontalScrollbarVisibility_mod;
        [Store] public bool inertia;
        [HideInInspector] public bool inertia_mod;
        [Store] public MovementType movementType;
        [HideInInspector] public bool movementType_mod;
        [Store] public ScrollRectEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_mod;
        [Store] public float scrollSensitivity;
        [HideInInspector] public bool scrollSensitivity_mod;
        [Store] public Vector2 velocity;
        [HideInInspector] public bool velocity_mod;
        [Store] public bool vertical;
        [HideInInspector] public bool vertical_mod;
        [Store] public Scrollbar verticalScrollbar;
        [HideInInspector] public bool verticalScrollbar_mod;
        [Store] public float verticalScrollbarSpacing;
        [HideInInspector] public bool verticalScrollbarSpacing_mod;
        [Store] public ScrollbarVisibility verticalScrollbarVisibility;
        [HideInInspector] public bool verticalScrollbarVisibility_mod;
        [Store] public RectTransform viewport;
        [HideInInspector] public bool viewport_mod;
    }
}

