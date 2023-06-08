//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace Studio.Common.Ui
{
    using System;
    using mulova.switcher;
    using TMPro;
    using UnityEngine;

    [Serializable]
    public class TextMeshProUGUIData : TMP_TextData
    {
        public override Type srcType => typeof(TextMeshProUGUI);

        [Store] public Vector4 maskOffset;
        [HideInInspector] public bool maskOffset_mod;
    }
}
