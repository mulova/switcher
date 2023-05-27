//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    public class SwitcherDisposer : System.IDisposable
    {
        private Switcher s;
        private object key;

        public SwitcherDisposer(Switcher s, object key)
        {
            this.s = s;
            this.key = key;
        }

        public void Dispose()
        {
            if (s != null)
            {
                s.Apply(key);
            }
        }
    }
}