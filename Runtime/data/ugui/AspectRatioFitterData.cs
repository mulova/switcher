//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEngine;
    using UnityEngine.UI;
    using static UnityEngine.UI.AspectRatioFitter;

    public class AspectRatioFitterData : MonoData<AspectRatioFitter>
    {
        [Store] public AspectMode aspectMode;
        [HideInInspector] public bool aspectMode_mod;
        [Store] public float aspectRatio;
        [HideInInspector] public bool aspectRatio_mod;
    }
}

