namespace DefaultNamespace.Utils
{
	public struct Vector4Int
	{
		public int X { get; }
		public int Y { get; }
		public int Z { get; }
		public int W { get; }

		public Vector4Int(int x, int y, int z, int w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
	}
}