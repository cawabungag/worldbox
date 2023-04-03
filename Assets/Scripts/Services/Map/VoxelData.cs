using Game.Services.MapGenerator;
using UnityEngine;

namespace Services.Map
{
	public struct VoxelData
	{
		public Vector3Int Position;
		public VoxelType Type;

		public VoxelData(Vector3Int position, VoxelType type)
		{
			Position = position;
			Type = type;
		}
	}
}