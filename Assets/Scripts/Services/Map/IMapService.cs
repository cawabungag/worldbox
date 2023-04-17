using Db.Brush;
using UnityEngine;

namespace Services.Map
{
	public interface IMapService
	{
		void AddVoxel(VoxelData voxelData);
		bool IsTransparent(int x, int y, int z);
		int[] GetVoxelEntities(Vector2Int[] cells);
		int[] GetVoxelEntities(Vector2Int inputPoint, Brush brush);
		Rect GetMapRect();
	}
}