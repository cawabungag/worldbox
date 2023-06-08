using UnityEngine;

namespace DefaultNamespace.Utils
{
	public static class VectorExtensions
	{
		public static Vector2 ToXZ(this Vector3 original)
		{
			return new Vector2(original.x, original.z);
		}
	}
}