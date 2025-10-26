//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher.foundation
{
	using System.Collections.Generic;

	public static class HashSetEx
	{
		public static void AddAll<T>(this HashSet<T> hashSet, IEnumerable<T> objs) {
			foreach (T t in objs) {
				hashSet.Add(t);
			}
		}

		public static void RemoveAll<T>(this HashSet<T> hashSet, IEnumerable<T> objs) {
			foreach (T t in objs) {
				hashSet.Remove(t);
			}
		}

	}
}

