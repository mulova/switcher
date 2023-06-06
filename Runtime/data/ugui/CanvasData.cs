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
        [HideInInspector] public bool renderMode_IsSet;
        [Store] public float scaleFactor;
        [HideInInspector] public bool scaleFactor_IsSet;
        [Store] public float referencePixelsPerUnit;
        [HideInInspector] public bool referencePixelsPerUnit_IsSet;
        [Store] public bool overridePixelPerfect;
        [HideInInspector] public bool overridePixelPerfect_IsSet;
        [Store] public bool pixelPerfect;
        [HideInInspector] public bool pixelPerfect_IsSet;
        [Store] public float planeDistance;
        [HideInInspector] public bool planeDistance_IsSet;
        [Store] public bool overrideSorting;
        [HideInInspector] public bool overrideSorting_IsSet;
        [Store] public int sortingOrder;
        [HideInInspector] public bool sortingOrder_IsSet;
        [Store] public int targetDisplay;
        [HideInInspector] public bool targetDisplay_IsSet;
        [Store] public int sortingLayerID;
        [HideInInspector] public bool sortingLayerID_IsSet;
        [Store] public AdditionalCanvasShaderChannels additionalShaderChannels;
        [HideInInspector] public bool additionalShaderChannels_IsSet;
        [Store] public string sortingLayerName;
        [HideInInspector] public bool sortingLayerName_IsSet;
        [Store] public Camera worldCamera;
        [HideInInspector] public bool worldCamera_IsSet;
        [Store] public float normalizedSortingGridSize;
        [HideInInspector] public bool normalizedSortingGridSize_IsSet;
    }
}

