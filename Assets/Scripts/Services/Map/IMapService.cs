using System.Collections.Generic;
using Db.Brush;
using UnityEngine;

namespace Services.Map
{
	public interface IMapService
	{
		bool IsTransparent(int x, int y, int z);
		List<int> GetVoxelEntities(Vector2Int inputPoint, Brush brush);
		int GetVoxelEntity(Vector2Int inputPoint);
	}
}