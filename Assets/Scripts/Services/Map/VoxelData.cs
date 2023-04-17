using Game.Services.MapGenerator;
using UnityEngine;

namespace Services.Map
{
	public struct VoxelData
	{
		public Vector3Int Position;
		public Vector2Int CellPosition;
		public VoxelType Type;

		public VoxelData(Vector3Int position, VoxelType type, Vector2Int cellPostiion)
		{
			Position = position;
			Type = type;
			CellPosition = cellPostiion;
		}
	}
}