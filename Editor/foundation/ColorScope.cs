//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher.foundation
{
    using System;
    using UnityEngine;

    public class ColorScope : IDisposable
    {
        private Color old;
        public ColorScope(Color c, bool apply = true)
        {
            old = GUI.color;
            if (apply)
            {
                GUI.color = c;
            }
        }

        public void Dispose()
        {
            GUI.color = old;
        }
    }
}
