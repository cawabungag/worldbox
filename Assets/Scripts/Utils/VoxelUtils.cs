using Game.Services.MapGenerator;

namespace DefaultNamespace.Utils
{
	public static class VoxelUtils
	{
		public static VoxelType ToVoxelType(this int height)
		{
			switch (height)
			{
				case 0:
					return VoxelType.GroundWater;
				case 1:
					return VoxelType.Water;
				case 2:
					return VoxelType.Sand;
				case 3:
					return VoxelType.Plain;
				case 4:
					return VoxelType.Forest;
				case 5:
					return VoxelType.Rock;
			}

			if (height > 5)
				return VoxelType.Rock;

			return VoxelType.None;
		}
	}
}