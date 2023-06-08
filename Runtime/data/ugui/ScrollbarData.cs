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
    using static UnityEngine.UI.Scrollbar;

    [Serializable]
    public class ScrollbarData : SelectableData<Scrollbar>
    {
        [Store] public Direction direction;
        [HideInInspector] public bool direction_mod;
        [Store] public RectTransform handleRect;
        [HideInInspector] public bool handleRect_mod;
        [Store] public int numberOfSteps;
        [HideInInspector] public bool numberOfSteps_mod;
        [Store] public ScrollEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_mod;
        [Store] public float size;
        [HideInInspector] public bool size_mod;
        [Store] public float value;
        [HideInInspector] public bool value_mod;
    }
}

