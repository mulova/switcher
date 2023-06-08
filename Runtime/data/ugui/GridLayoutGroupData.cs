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
    using static UnityEngine.UI.GridLayoutGroup;

    [Serializable]
    public class GridLayoutGroupData : LayoutGroupData
    {
        public override Type srcType => typeof(GridLayoutGroup);

        [Store] public Corner startCorner;
        [HideInInspector] public bool startCorner_mod;
        [Store] public Axis startAxis;
        [HideInInspector] public bool startAxis_mod;
        [Store] public Vector2 cellSize;
        [HideInInspector] public bool cellSize_mod;
        [Store] public Vector2 spacing;
        [HideInInspector] public bool spacing_mod;
        [Store] public Constraint constraint;
        [HideInInspector] public bool constraint_mod;
        [Store] public int constraintCount;
        [HideInInspector] public bool constraintCount_mod;
    }
}

