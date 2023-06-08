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
    public class CanvasGroupData : MonoData<CanvasGroup>
    {
        [Store] public float alpha;
        [HideInInspector] public bool alpha_mod;
        [Store] public bool interactable;
        [HideInInspector] public bool interactable_mod;
        [Store] public bool blocksRaycasts;
        [HideInInspector] public bool blocksRaycasts_mod;
        [Store] public bool ignoreParentGroups;
        [HideInInspector] public bool ignoreParentGroups_mod;
    }
}

