using System.Collections.Generic;
using Db.Brush;
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

		public MapService(EcsWorld world)
		{
			_poolVoxelPosition = world.GetPool<VoxelPositionComponent>();
			_filterChunks = world.Filter<ChunkComponent>().End();
			_poolChunks = world.GetPool<ChunkComponent>();
			_poolVoxelsInChunk = world.GetPool<VoxelsInChunkComponent>();
		}

		public void AddVoxel(VoxelData voxelData)
		{
			_voxelDataBuffer.Add(voxelData.Position, voxelData);
		}

		public bool IsTransparent(int x, int y, int z)
		{
			var pos = new Vector3Int(x, y, z);
			var isTransparent = !_voxelDataBuffer.TryGetValue(pos, out _);
			return isTransparent;
		}

		public int[] GetVoxelEntities(Vector2Int[] cells)
		{
			Profiler.BeginSample("GetVoxelEntities");
			_entitiesBuffer.Clear();

			foreach (var inputCell in cells)
			{
				foreach (var chunk in _filterChunks)
				{
					var bound = _poolChunks.Get(chunk).Value;
					if (inputCell.x > bound.x
						&& inputCell.y > bound.y
						&& inputCell.x < bound.z
						&& inputCell.y < bound.w)
					{
						var voxelsInChunk = _poolVoxelsInChunk.Get(chunk).Value;
						foreach (var voxel in voxelsInChunk)
						{
							var cellPosition = _poolVoxelPosition.Get(voxel).Value;
							foreach (var cell in cells)
							{
								if (cellPosition != cell)
									continue;

								_entitiesBuffer.Add(voxel);
							}
						}
					}
				}
			}

			var voxelEntities = _entitiesBuffer.ToArray();
			Profiler.EndSample();
			return voxelEntities;
		}

		public int[] GetVoxelEntities(Vector2Int inputPoint, Brush brush)
		{
			var cells = ConvertToCells(inputPoint, brush);
			var entities = GetVoxelEntities(cells);
			return entities;
		}

		public Rect GetMapRect()
		{
			throw new System.NotImplementedException();
		}

		private Vector2Int[] ConvertToCells(Vector2Int inputPoint, Brush brush)
		{
			Profiler.BeginSample("ConvertToCells");
			var list = new List<Vector2Int>();
			var startX = inputPoint.x - brush.Width / 2;
			var startY = inputPoint.y + brush.Height / 2;

			for (var y = 0; y < brush.Height; y++)
			{
				for (var x = 0; x < brush.Width; x++)
				{
					if (!brush.GetPoint(x, y))
						continue;

					if (expr)
					{
						
					}

					var tilePosition = new Vector2Int(startX + x, startY - y);
					list.Add(tilePosition);
				}
			}

			var convertToCells = list.ToArray();
			Profiler.EndSample();
			return convertToCells;
		}
	}
}