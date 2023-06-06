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
    using static UnityEngine.UI.Toggle;

    [Serializable]
    public class ToggleData : SelectableData<Toggle>
    {
        [Store] public Graphic graphic;
        [HideInInspector] public bool graphic_IsSet;
        [Store] public ToggleGroup group;
        [HideInInspector] public bool group_IsSet;
        [Store] public bool isOn;
        [HideInInspector] public bool isOn_IsSet;
        [Store] public ToggleEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_IsSet;
        [Store] public ToggleTransition toggleTransition;
        [HideInInspector] public bool toggleTransition_IsSet;
    }
}

