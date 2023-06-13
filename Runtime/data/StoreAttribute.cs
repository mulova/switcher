//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class StoreAttribute : Attribute
    {
        public bool manual;

        public StoreAttribute(bool manual = false)
        {
            this.manual = manual;
        }
    }
}

