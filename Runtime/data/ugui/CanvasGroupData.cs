//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Serializable, Preserve]
    public class CanvasGroupData : MonoData<CanvasGroup>
    {
        [Store, Preserve] public float alpha;
        [HideInInspector, Preserve] public bool alpha_IsSet;
        [Store, Preserve] public bool interactable;
        [HideInInspector, Preserve] public bool interactable_IsSet;
        [Store, Preserve] public bool blocksRaycasts;
        [HideInInspector, Preserve] public bool blocksRaycasts_IsSet;
        [Store, Preserve] public bool ignoreParentGroups;
        [HideInInspector, Preserve] public bool ignoreParentGroups_IsSet;
    }
}

