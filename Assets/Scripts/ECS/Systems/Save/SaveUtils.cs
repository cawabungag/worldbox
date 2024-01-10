using UnityEngine;

namespace DefaultNamespace.Systems.Save
{
	public static class SaveUtils
	{
		public static Vector2IntSerializable AsSerializable(this Vector2Int value)
		{
			return new Vector2IntSerializable(value.x, value.y);
		}
		
		public static Vector3IntSerializable AsSerializable(this Vector3Int value)
		{
			return new Vector3IntSerializable(value.x, value.y, value.z);
		}
	}
}