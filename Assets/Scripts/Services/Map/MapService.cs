using System.Collections.Generic;
using UnityEngine;

namespace Services.Map
{
	public class MapService : IMapService
	{
		private readonly Dictionary<Vector3Int, VoxelData> _voxelDataBuffer = new();

		public void AddVoxel(VoxelData voxelData)
		{
			_voxelDataBuffer.Add(voxelData.Position, voxelData);
		}

		public bool IsTransparent(int x, int y, int z)
		{
			var pos = new Vector3Int(x, y, z);
			return !_voxelDataBuffer.TryGetValue(pos, out _);
		}
	}
}