using UnityEngine;

namespace DefaultNamespace.Utils
{
	public static class VoxelUVUtils
	{
		public static Vector2 grassA = new(0.25f, 1);
		public static Vector2 grassb = new(0f, 0.75f);
		public static Vector2 grassc = new(0f, 1);
		public static Vector2 grassd = new(0f, 0.75f);
	
		public static Vector2 waterA = new(-0.09f, 1);
		public static Vector2 waterb = new(0f, 0.75f);
		public static Vector2 waterc = new(0f, 1);
		public static Vector2 waterd = new(0f, 0.75f);
	
		public static Vector2 groundA = new(0f, 1);
		public static Vector2 groundb = new(0f, 0.75f);
		public static Vector2 groundc = new(0f, 1);
		public static Vector2 groundd = new(0f, 0.75f);
	
		public static Vector2 stoneA = new(0.5f, 1);
		public static Vector2 stoneb = new(0.5f, 0.75f);
		public static Vector2 stonec = new(0.25f, 1);
		public static Vector2 stoned = new(0.25f, 0.75f);
	
		public static Vector2 sandA = new(0.5f, 1);
		public static Vector2 sandb = new(0.5f, 0.75f);
		public static Vector2 sandc = new(0.25f, 1);
		public static Vector2 sandd = new(0.25f, 0.75f);
	
		public static Vector2 forestA = new(0.5f, 0.9f);
		public static Vector2 forestb = new(0.75f, 1f);
		public static Vector2 forestc = new(0.75f, 1);
		public static Vector2 forestd = new(0.5f, 0.75f);
	}
}