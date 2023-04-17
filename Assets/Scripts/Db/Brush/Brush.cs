namespace Db.Brush
{
	public class Brush
	{
		public int Height;
		public int Width;
		public bool[] Points;
		public bool GetPoint(int x, int y) => Points[y * Width + x];

		public Brush(int height, int width, bool[] points)
		{
			Height = height;
			Width = width;
			Points = points;
		}
	}
}