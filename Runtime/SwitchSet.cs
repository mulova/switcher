//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace mulova.switcher
{
    [Serializable]
    public class SwitchSet : ICloneable
    {
        [SerializeReference] [SubclassSelector] public List<ICompData> data = new List<ICompData>();
        public UnityEvent action;
        [EnumPopup("enumType")] public string name;

        public bool isValid
        {
            get
            {
                return !string.IsNullOrEmpty(name);
            }
        }

        public object Clone()
        {
            SwitchSet e = new SwitchSet();
            e.name = this.name;
            e.data = new List<ICompData>(data);
            return e;
        }

        public override string ToString()
        {
            return name;
        }
    }
}