//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DriveAttribute : Attribute
    {
        public readonly Type drivenType;
        public readonly string drivenName;

        public DriveAttribute(Type drivenType, string drivenName)
        {
            this.drivenType = drivenType;
            this.drivenName = drivenName;
        }
    }
}

