//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher.foundation
{
    using System;
    using System.Collections.Generic;

    public static class EnumEx
    {
        private static readonly Dictionary<Enum, string> cache = new Dictionary<Enum, string>();
        private static readonly Dictionary<Enum, string> lowerCache = new Dictionary<Enum, string>();

        public static string ToStringCached(this Enum value)
        {
            if (!cache.TryGetValue(value, out var str))
            {
                cache[value] = str = value.ToString();
            }
            return str;
        }

        public static string ToStringLowerCached(this Enum value)
        {
            if (!lowerCache.TryGetValue(value, out var str))
            {
                lowerCache[value] = str = value.ToString().ToLower();
            }
            return str;
        }
    }
}
