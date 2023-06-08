//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static TMPro.TMP_Dropdown;

    [Serializable]
    public class TMP_DropDownData : SelectableData<TMP_Dropdown>
    {
        [Store] public float alphaFadeSpeed;
        [HideInInspector] public bool alphaFadeSpeed_mod;
        [Store] public Image captionImage;
        [HideInInspector] public bool captionImage_mod;
        [Store] public TMP_Text captionText;
        [HideInInspector] public bool captionText_mod;
        [Store] public Image itemImage;
        [HideInInspector] public bool _mod;
        [Store] public TMP_Text itemText;
        [HideInInspector] public bool itemText_mod;
        [Store] public DropdownEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_mod;
        [Store] public List<OptionData> options;
        [HideInInspector] public bool options_mod;
        [Store] public Graphic placeholder;
        [HideInInspector] public bool placeholder_mod;
        [Store] public RectTransform template;
        [HideInInspector] public bool template_mod;
        [Store] public int value;
        [HideInInspector] public bool value_mod;
    }
}

