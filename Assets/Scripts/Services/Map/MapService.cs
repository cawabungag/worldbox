using System.Collections.Generic;
using Db.Brush;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Profiling;

namespace Services.Map
{
	public class MapService : IMapService
	{
		private readonly List<int> _entitiesBuffer = new();
		private readonly Dictionary<Vector3Int, VoxelData> _voxelDataBuffer = new();

		private EcsPool<VoxelPositionComponent> _poolVoxelPosition;
		private EcsFilter _filterChunks;
		private EcsPool<ChunkComponent> _poolChunks;
		private EcsPool<VoxelsInChunkComponent> _poolVoxelsInChunk;
		private Rect _mapRect;
		private List<Vector2Int> _cellsBuffer = new();
		private EcsWorld _world;

		public MapService(EcsWorld world)
		{
			_world = world;
			_poolVoxelPosition = world.GetPool<VoxelPositionComponent>();
			_filterChunks = world.Filter<ChunkComponent>().End();
			_poolChunks = world.GetPool<ChunkComponent>();
			_poolVoxelsInChunk = world.GetPool<VoxelsInChunkComponent>();
			_mapRect = new Rect(Vector2.zero, new Vector2(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE));
		}

		public bool IsTransparent(int x, int y, int z)
		{
			Profiler.BeginSample("IsTransparent");
			var pos = new Vector3Int(x, y, z);
			var isTransparent = !_voxelDataBuffer.TryGetValue(pos, out _);
			Profiler.EndSample();
			return isTransparent;
		}

		private List<int> GetVoxelEntities(List<Vector2Int> cells)
		{
			var gridGraph = _world.GetUnique<MapGraphComponent>().Value;
			Profiler.BeginSample("GetVoxelEntities");
			_entitiesBuffer.Clear();

			foreach (var inputCell in cells)
			{
				var halfWidth = WorldUtils.WORLD_SIZE / 2;
				var halfHeight = WorldUtils.WORLD_SIZE / 2;
				var cellPos = inputCell - new Vector2Int(WorldUtils.WORLD_SIZE / 2, WorldUtils.WORLD_SIZE / 2);
				var fixedPos = new Vector2Int(cellPos.y, cellPos.x);
				var entity = gridGraph.GetEntity(fixedPos.x + halfWidth, fixedPos.y + halfHeight);
				_entitiesBuffer.Add(entity);
			}

			// foreach (var inputCell in cells)
			// {
			// 	//TODO Need refactoring position
			// 	var cellPos = inputCell - new Vector2Int(WorldUtils.WORLD_SIZE / 2, WorldUtils.WORLD_SIZE / 2);
			// 	var fixedPos = new Vector2Int(cellPos.y, cellPos.x);
			//
			// 	foreach (var chunk in _filterChunks)
			// 	{
			// 		var bound = _poolChunks.Get(chunk).Value;
			// 		if (!IsInBound(inputCell, bound))
			// 			continue;
			//
			// 		var voxelsInChunk = _poolVoxelsInChunk.Get(chunk).Value;
			// 		foreach (var voxel in voxelsInChunk)
			// 		{
			// 			var cellPosition = _poolVoxelPosition.Get(voxel).Value;
			// 			if (cellPosition == fixedPos)
			// 				_entitiesBuffer.Add(voxel);
			// 		}
			// 	}
			// }

			return _entitiesBuffer;
		}

		private static bool IsInBound(Vector2Int inputCell, Vector4 bound)
		{
			return inputCell.x >= bound.x && inputCell.y >= bound.y && inputCell.x <= bound.z
					&& inputCell.y <= bound.w;
		}

		public List<int> GetVoxelEntities(Vector2Int inputPoint, Brush brush)
		{
			var cells = ConvertToCells(inputPoint, brush);
			var entities = GetVoxelEntities(cells);
			return entities;
		}

		public int GetVoxelEntity(Vector2Int inputPoint)
		{
			//TODO Need refactoring position
			var cellPos = inputPoint - new Vector2Int(WorldUtils.WORLD_SIZE / 2, WorldUtils.WORLD_SIZE / 2);
			var fixedPos = new Vector2Int(cellPos.y, cellPos.x);

			foreach (var chunk in _filterChunks)
			{
				var bound = _poolChunks.Get(chunk).Value;
				if (!IsInBound(inputPoint, bound))
					continue;

				var voxelsInChunk = _poolVoxelsInChunk.Get(chunk).Value;
				foreach (var voxel in voxelsInChunk)
				{
					var cellPosition = _poolVoxelPosition.Get(voxel).Value;
					if (cellPosition == fixedPos)
						return voxel;
				}
			}

			return -1;
		}

		private List<Vector2Int> ConvertToCells(Vector2Int inputPoint, Brush brush)
		{
			Profiler.BeginSample("ConvertToCells");
			_cellsBuffer.Clear();
			var startX = inputPoint.x - brush.Width / 2;
			var startY = inputPoint.y + brush.Height / 2;

			for (var y = 0; y < brush.Height; y++)
			{
				for (var x = 0; x < brush.Width; x++)
				{
					var tilePosition = new Vector2Int(startX + x, startY - y);
					if (!_mapRect.Contains(tilePosition))
						continue;

					if (!brush.GetPoint(x, y))
						continue;

					_cellsBuffer.Add(tilePosition);
				}
			}

			Profiler.EndSample();
			return _cellsBuffer;
		}
	}
}