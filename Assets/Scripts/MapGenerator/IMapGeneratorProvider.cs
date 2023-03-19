using System;

namespace Game.Services.MapGenerator
{
	public interface IMapGeneratorProvider
	{
		ETileType[] GenerateGround(int width, int height);
	}
	
	[Flags]
	public enum ETileType : byte
	{
		GroundWater = 1,
		Water = 2,
		Sand = 3,
		Plain = 5,
		Forest = 7,
		Rock = 8,
	}
}