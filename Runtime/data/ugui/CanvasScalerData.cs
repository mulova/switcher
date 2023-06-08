//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEngine;
    using UnityEngine.UI;
    using static UnityEngine.UI.CanvasScaler;

    public class CanvasScalerData : MonoData<CanvasScaler>
    {
        [Store] public float defaultSpriteDPI;
        [HideInInspector] public bool defaultSpriteDPI_mod;
        [Store] public float dynamicPixelsPerUnit;
        [HideInInspector] public bool dynamicPixelsPerUnit_mod;
        [Store] public float fallbackScreenDPI;
        [HideInInspector] public bool fallbackScreenDPI_mod;
        [Store] public float matchWidthOrHeight;
        [HideInInspector] public bool matchWidthOrHeight_mod;
        [Store] public Unit physicalUnit;
        [HideInInspector] public bool physicalUnit_mod;
        [Store] public float referencePixelsPerUnit;
        [HideInInspector] public bool referencePixelsPerUnit_mod;
        [Store] public Vector2 referenceResolution;
        [HideInInspector] public bool referenceResolution_mod;
        [Store] public float scaleFactor;
        [HideInInspector] public bool scaleFactor_mod;
        [Store] public ScreenMatchMode screenMatchMode;
        [HideInInspector] public bool screenMatchMode_mod;
        [Store] public CanvasScaler.ScaleMode uiScaleMode;
        [HideInInspector] public bool uiScaleMode_mod;
    }
}

