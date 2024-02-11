using Db.Brush;
using DefaultNamespace.Components.Plant;
using DefaultNamespace.Components.Unit;
using DefaultNamespace.Db.Unit;
using ECS.Components.Map;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;

namespace Tools.Unit
{
	public abstract class BaseSpawnUnitStrategy : IUseToolStrategy
	{
		private readonly IMapService _mapService;
		private readonly UnitsData _unitsData;
		private readonly EcsWorld _world;
		private EcsPool<PoolIndexComponent> _poolIndexPool;
		private EcsPool<UnitTypeComponent> _unitTypePool;
		private EcsPool<PositionComponent> _positionComponent;
		private EcsPool<VoxelPositionComponent> _voxelPostion;
		private EcsPool<VoxelTypeComponent> _voxelTypePool;

		public abstract ToolType ToolType { get; }
		public abstract int PoolIndex { get; }
		public abstract UnitType UnitType { get; }

		public BaseSpawnUnitStrategy(IMapService mapService, UnitsData unitsData, EcsWorld world)
		{
			_mapService = mapService;
			_unitsData = unitsData;
			_world = world;
			_poolIndexPool = _world.GetPool<PoolIndexComponent>();
			_unitTypePool = _world.GetPool<UnitTypeComponent>();
			_positionComponent = _world.GetPool<PositionComponent>();
			_voxelPostion = _world.GetPool<VoxelPositionComponent>();
			_voxelTypePool = _world.GetPool<VoxelTypeComponent>();
		}
		
		public void UseBrush(Vector3 worldTouchPoint, BrushType brushType, int brushSize)
		{
		}

		public void SingleCellUse(Vector3 worldTouchPoint)
		{
			var inputPoint = new Vector2Int((int) worldTouchPoint.x, (int) worldTouchPoint.y);
			var entity = _mapService.GetVoxelEntity(inputPoint);
			if (entity == -1)
				return;
			
			var unitEntity = _world.NewEntity();
			_poolIndexPool.Add(unitEntity).Value = PoolIndex;
			_unitTypePool.Add(unitEntity).Value = UnitType;
			var position = _voxelPostion.Get(entity).Value;
			var voxel = _voxelTypePool.Get(entity).Value;
			var unitPosition = new Vector3Int(position.x, (int) voxel, position.y);
			_positionComponent.Add(unitEntity).Value = unitPosition;
		}
	}
}