//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    // NOTE: Equals() and GetHashCode() must be implemented
    public interface ICompData
    {
        Type srcType { get; }
        bool active { get; }
        Component target { get; set; }
        void ApplyTo(Component c);
        void Collect(Component c, bool changedOnly);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="v0">the first component data</param>
        /// <param name="vi"></param>
        /// <returns></returns>
        bool MemberEquals(MemberControl member, object val0, object vali);

        List<MemberControl> ListAttributedMembers();
    }
}

