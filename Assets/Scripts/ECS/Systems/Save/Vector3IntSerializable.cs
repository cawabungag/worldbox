using System;

namespace DefaultNamespace.Systems.Save
{
	[Serializable]
	public struct Vector3IntSerializable
	{
		public int X;
		public int Y;
		public int Z;

		public Vector3IntSerializable(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}