//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine.UI;

    [Serializable]
    public class HorizontalLayoutGroupData : HorizontalOrVerticalLayoutGroupData
    {
        public override Type srcType => typeof(HorizontalLayoutGroup);
    }
}

