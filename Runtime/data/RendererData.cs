//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    [Serializable]
    public class RendererData : MonoData<Renderer>
    {
        [Store] public ShadowCastingMode shadowCastingMode;
        [HideInInspector] public bool shadowCastingMode_mod;
        [Store] public bool receiveShadows;
        [HideInInspector] public bool receiveShadows_mod;
        [Store] public MotionVectorGenerationMode motionVectorGenerationMode;
        [HideInInspector] public bool motionVectorGenerationMode_mod;
        [Store] public LightProbeUsage lightProbeUsage;
        [HideInInspector] public bool lightProbeUsage_mod;
        [Store] public ReflectionProbeUsage reflectionProbeUsage;
        [HideInInspector] public bool reflectionProbeUsage_mod;
        [Store] public uint renderingLayerMask;
        [HideInInspector] public bool renderingLayerMask_mod;
        [Store] public int rendererPriority;
        [HideInInspector] public bool rendererPriority_mod;
        [Store] public int sortingLayerID;
        [HideInInspector] public bool sortingLayerID_mod;
        [Store] public int sortingOrder;
        [HideInInspector] public bool sortingOrder_mod;
        [Store] public bool allowOcclusionWhenDynamic;
        [HideInInspector] public bool allowOcclusionWhenDynamic_mod;
        [Store] public Material[] sharedMaterials;
        [HideInInspector] public bool sharedMaterials_mod;
    }
}

