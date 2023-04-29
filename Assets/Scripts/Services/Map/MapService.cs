using System.Collections.Generic;
using Db.Brush;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using UnityEngine;

namespace Services.Map
{
	public class MapService : IMapService
	{
		private readonly List<int> _entitiesBuffer = new ();
		private readonly Dictionary<Vector3Int, VoxelData> _voxelDataBuffer = new();
		private readonly Dictionary<Vector2Int, (VoxelType voxelType, int entity)> _higestVoxelOnCellBuffer = new();
		
		private EcsPool<VoxelPositionComponent> _poolVoxelPosition;
		private EcsPool<VoxelTypeComponent> _poolVoxelType;
		private EcsFilter _filterVoxelPosition;

		public MapService(EcsWorld world)
		{
			_poolVoxelPosition = world.GetPool<VoxelPositionComponent>();
			_poolVoxelType = world.GetPool<VoxelTypeComponent>();
			
			_filterVoxelPosition = world.Filter<VoxelPositionComponent>().End();
		}
		
		public void AddVoxel(VoxelData voxelData)
		{
			_voxelDataBuffer.Add(voxelData.Position, voxelData);
		}

		public bool IsTransparent(int x, int y, int z)
		{
			var pos = new Vector3Int(x, y, z);
			return !_voxelDataBuffer.TryGetValue(pos, out _);
		}

		public int[] GetVoxelEntities(Vector2Int[] cells)
		{
			_entitiesBuffer.Clear();
			_higestVoxelOnCellBuffer.Clear();
			
			var rawEntities = _filterVoxelPosition.GetRawEntities();
			
			for (int i = 0; i < rawEntities.Length; i++)
			{
				var entity = rawEntities[i];
				var cellPosition = _poolVoxelPosition.Get(entity).Value;
				var voxelType = _poolVoxelType.Get(entity).Value;
				foreach (var cell in cells)
				{
					if (cellPosition == cell)
					{
						if (_higestVoxelOnCellBuffer.TryGetValue(cellPosition, out var maxVoxelTypeInBuffer))
						{
							if (voxelType > maxVoxelTypeInBuffer.Item1)
								_higestVoxelOnCellBuffer[cellPosition] = (voxelType, entity);
						}
						else
						{
							_higestVoxelOnCellBuffer.Add(cellPosition, (voxelType, entity));
						}
					}
				}
			}

			foreach (var tuple in _higestVoxelOnCellBuffer)
			{
				_entitiesBuffer.Add(tuple.Value.entity);
			}

			return _entitiesBuffer.ToArray();
		}

		public int[] GetVoxelEntities(Vector2Int inputPoint, Brush brush)
		{
			var cells = ConvertToCells(inputPoint, brush);
			var entities = GetVoxelEntities(cells);
			return entities;
		}

		public Rect GetMapRect()
		{
			var mapRect = new Rect(Vector2.zero, new Vector2(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE));
			return mapRect;
		}

		private Vector2Int[] ConvertToCells(Vector2Int inputPoint, Brush brush)
		{
			var list = new List<Vector2Int>();
			var startX = inputPoint.x - brush.Width / 2;
			var startY = inputPoint.y + brush.Height / 2;

			for (var y = 0; y < brush.Height; y++)
			{
				for (var x = 0; x < brush.Width; x++)
				{
					if (!brush.GetPoint(x, y))
						continue;

					var tilePosition = new Vector2Int(startX + x, startY - y);
					var hasCell = HasCell(tilePosition);

					if (!hasCell)
						continue;

					list.Add(tilePosition);
				}
			}

			return list.ToArray();
		}

		private bool HasCell(Vector2Int tilePosition)
		{
			foreach (var item in _poolVoxelPosition.GetRawDenseItems())
			{
				if (item.Value == tilePosition)
				{
					return true;
				}
			}

			return false;
		}
	}
}