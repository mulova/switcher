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
    using static UnityEngine.UI.ContentSizeFitter;

    [Serializable]
    public class ContentSizeFitterData : MonoData<ContentSizeFitter>
    {
        [Store] public FitMode horizontalFit;
        [HideInInspector] public bool horizontalFit_mod;
        [Store] public FitMode verticalFit;
        [HideInInspector] public bool verticalFit_mod;
    }
}

