﻿//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEngine;

    public class EnumPopupAttribute : PropertyAttribute
    {
        public readonly string enumTypeVar;
        
        public EnumPopupAttribute(string enumVar)
        {
            this.enumTypeVar = enumVar;
        }
    }
}
