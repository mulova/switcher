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
    public class SwitcherEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class Case : ICloneable
    {
        [SerializeReference] public List<CompData> data = new List<CompData>();
        public SwitcherEvent action;
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

        public bool IsApplied()
        {
            return data.TrueForAll(d => d != null && d.IsApplied());
        }

        public object Clone()
        {
            Case c = new Case();
            c.name = this.name;
            c.data = new List<CompData>(data);
            return c;
        }

        public override string ToString()
        {
            return name;
        }
    }
}