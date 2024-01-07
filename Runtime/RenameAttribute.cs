//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using UnityEngine;

    public class RenameAttribute : PropertyAttribute
    {
        public readonly string name;
        public RenameAttribute(string name)
        {
            this.name = name;
        }
    }
}