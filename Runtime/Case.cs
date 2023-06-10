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
    public class Case : ICloneable
    {
#if SERIALIZE_REFERENCE_EXT
        [SubclassSelector] 
#endif
        [SerializeReference] public List<ICompData> data = new List<ICompData>();
        public UnityEvent action;
        [EnumPopup("enumType")] public string name;

        public bool showAction { get; set; } = false; // editor only
        internal bool hasAction => action != null && action.GetPersistentEventCount() > 0;

        public bool isValid
        {
            get
            {
                return !string.IsNullOrEmpty(name);
            }
        }

        public object Clone()
        {
            Case c = new Case();
            c.name = this.name;
            c.data = new List<ICompData>(data);
            return c;
        }

        public override string ToString()
        {
            return name;
        }
    }
}