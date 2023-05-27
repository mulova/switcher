//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
	using UnityEngine;

	public static class QuaternionEx {
		
		public static readonly Quaternion Y_45 = Quaternion.AngleAxis(45, Vector3.up);
		public static readonly Quaternion Y_90 = Quaternion.AngleAxis(90, Vector3.up);
		public static readonly Quaternion Y_135 = Quaternion.AngleAxis(135, Vector3.up);
		public static readonly Quaternion Y_180 = Quaternion.AngleAxis(180, Vector3.up);
		public static readonly Quaternion Y_225 = Quaternion.AngleAxis(225, Vector3.up);
		public static readonly Quaternion Y_MINUS_90 = Quaternion.AngleAxis(-90, Vector3.up);
		
		
		public static bool IsValid(this Quaternion q) {
			if (q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0)
				return false;
			return !(float.IsNaN(q.x) || float.IsNaN(q.x) || float.IsNaN(q.x) || float.IsNaN(q.x));
		}
		
		
		public static bool ApproximatelyEquals(this Quaternion first,  Quaternion second) {
			return (Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y)
				&& Mathf.Approximately(first.z, second.z) && Mathf.Approximately(first.w, second.w)) ||
				(Mathf.Approximately(first.x, -second.x) && Mathf.Approximately(first.y, -second.y)
					&& Mathf.Approximately(first.z, -second.z) && Mathf.Approximately(first.w, -second.w));
		}

		public static bool ApproximatelyEquals(this Quaternion first,  Quaternion second, float delta) {
			return Mathf.Abs(first.x - second.x) < delta
				&& Mathf.Abs(first.y - second.y) < delta
				&& Mathf.Abs(first.z - second.z) < delta
				&& Mathf.Abs(first.w - second.w) < delta;
		}
	}
}
