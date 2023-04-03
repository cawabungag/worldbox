using System;

namespace Game.Services.MapGenerator
{
	[Flags]
	public enum VoxelType : byte
	{
		None = 0,
		GroundWater = 1,
		Water = 2,
		Sand = 3,
		Plain = 5,
		Forest = 7,
		Rock = 8,
	}
}