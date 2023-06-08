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
    public class CanvasData : MonoData<Canvas>
    {
        [Store] public RenderMode renderMode;
        [HideInInspector] public bool renderMode_mod;
        [Store] public float scaleFactor;
        [HideInInspector] public bool scaleFactor_mod;
        [Store] public float referencePixelsPerUnit;
        [HideInInspector] public bool referencePixelsPerUnit_mod;
        [Store] public bool overridePixelPerfect;
        [HideInInspector] public bool overridePixelPerfect_mod;
        [Store] public bool pixelPerfect;
        [HideInInspector] public bool pixelPerfect_mod;
        [Store] public float planeDistance;
        [HideInInspector] public bool planeDistance_mod;
        [Store] public bool overrideSorting;
        [HideInInspector] public bool overrideSorting_mod;
        [Store] public int sortingOrder;
        [HideInInspector] public bool sortingOrder_mod;
        [Store] public int targetDisplay;
        [HideInInspector] public bool targetDisplay_mod;
        [Store] public int sortingLayerID;
        [HideInInspector] public bool sortingLayerID_mod;
        [Store] public AdditionalCanvasShaderChannels additionalShaderChannels;
        [HideInInspector] public bool additionalShaderChannels_mod;
        [Store] public string sortingLayerName;
        [HideInInspector] public bool sortingLayerName_mod;
        [Store] public Camera worldCamera;
        [HideInInspector] public bool worldCamera_mod;
        [Store] public float normalizedSortingGridSize;
        [HideInInspector] public bool normalizedSortingGridSize_mod;
    }
}

