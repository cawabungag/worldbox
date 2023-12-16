using System;

namespace Game.Services.MapGenerator
{
	[Serializable]
	public enum VoxelType : byte
	{
		GroundWater = 0,
		Water = 1,
		Sand = 2,
		Plain = 3,
		Forest = 4,
		Rock = 5,
	}
}