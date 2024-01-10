using System;

namespace DefaultNamespace.Systems.Save
{
	[Serializable]
	public struct Vector2IntSerializable
	{
		public int X;
		public int Y;

		public Vector2IntSerializable(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}