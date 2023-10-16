using UnityEngine;

namespace DefaultNamespace.Utils
{
	public static class VoxelUVUtils
	{
		//   2 --- 3
		//   |     |
		//   |     |
		//   0 --- 1
		
		// theUVs[2] = Vector2( 0, 1 );
		// theUVs[3] = Vector2( 1, 1 );
		// theUVs[0] = Vector2( 0, 0 );
		// theUVs[1] = Vector2( 1, 0 );

		//    2    3    0    1   Front
		//    6    7   10   11   Back
		//   19   17   16   18   Left
		//   23   21   20   22   Right
		//    4    5    8    9   Top
		//   15   13   12   14   Bottom
		
		public static Vector2 grassA = new(0f, 0.6f);
		public static Vector2 grassb = new(0.2f, 0.6f);
		public static Vector2 grassc = new(0f, 0.8f);
		public static Vector2 grassd = new(0.2f, 0.8f);
		
		public static Vector2 waterA = new(0f, 0f);
		public static Vector2 waterb = new(0.2f, 0f);
		public static Vector2 waterc = new(0f, 0.2f);
		public static Vector2 waterd = new(0.2f, 0.2f);
		
		public static Vector2 groundA = new(0.2f, 0);
		public static Vector2 groundb = new(0.4f, 0);
		public static Vector2 groundc = new(0.2f, 0.2f);
		public static Vector2 groundd = new(0.4f, 0.2f);
	
		public static Vector2 stoneA = new(0f, 0.4f);
		public static Vector2 stoneb = new(0.2f, 0.4f);
		public static Vector2 stonec = new(0f, 0.6f);
		public static Vector2 stoned = new(0.2f, 0.6f);
	
		public static Vector2 sandA = new(0f, 0.4f);
		public static Vector2 sandb = new(0.2f, 0.4f);
		public static Vector2 sandc = new(0f, 0.6f);
		public static Vector2 sandd = new(0.2f, 0.6f);
	
		public static Vector2 forestA = new(0f, 0.2f);
		public static Vector2 forestb = new(0.2f, 0.2f);
		public static Vector2 forestc = new(0f, 0.4f);
		public static Vector2 forestd = new(0.2f, 0.4f);
	}
}