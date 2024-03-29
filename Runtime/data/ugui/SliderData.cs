﻿//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using static UnityEngine.UI.Slider;

    [Serializable]
    public class SliderData : SelectableData<Slider>
    {
        [Store] public Direction direction;
        [HideInInspector] public bool direction_mod;
        [Store] public RectTransform fillRect;
        [HideInInspector] public bool fillRect_mod;
        [Store] public RectTransform handleRect;
        [HideInInspector] public bool handleRect_mod;
        [Store] public float maxValue;
        [HideInInspector] public bool maxValue_mod;
        [Store] public float minValue;
        [HideInInspector] public bool minValue_mod;
        [Store] public SliderEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_mod;
        [Store] public float value;
        [HideInInspector] public bool value_mod;
        [Store] public bool wholeNumbers;
        [HideInInspector] public bool wholeNumbers_mod;
    }
}

