//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

using System;

namespace mulova.switcher
{
    [Serializable]
    public class SwitchPreset : ICloneable
    {
        public string presetName;
        public string[] keys = new string[0];

        public object Clone()
        {
            SwitchPreset p = new SwitchPreset();
            p.keys = (string[]) keys.Clone();
            return p;
        }
    }
}