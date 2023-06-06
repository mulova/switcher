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
    using static UnityEngine.UI.GraphicRaycaster;

    [Serializable]
    public class GraphicRaycasterData : MonoData<GraphicRaycaster>
    {
        [Store] public LayerMask blockingMask;
        [HideInInspector] public bool blockingMask_IsSet;
        [Store] public bool ignoreReversedGraphics;
        [HideInInspector] public bool ignoreReversedGraphics_IsSet;
        [Store] public BlockingObjects blockingObjects;
        [HideInInspector] public bool blockingObjects_IsSet;
    }
}

