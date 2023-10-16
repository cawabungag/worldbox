using System.Collections.Generic;
using Db.Brush;
using Db.Plant;
using ECS.Components.Map;
using Leopotam.EcsLite;
using Services.Map;
using Tools;
using UnityEngine;

namespace DefaultNamespace.Components.Plant
{
	public abstract class BasePlantStrategy : BaseBrushToolStrategy
	{
		private readonly PlantsData _plantsData;

		private EcsWorld _world;
		private PlantData _plantData;

		private readonly EcsPool<VoxelPositionComponent> _poolVoxelPosition;
		private readonly EcsPool<VoxelTypeComponent> _poolVoxelTypeCompoenent;
		private readonly EcsPool<PlantPositionComponent> _poolPlantPosiiton;
		private readonly EcsPool<PlantTypeComponent> _poolPlantTypeComponent;
		private readonly EcsPool<PlantPoolIndexComponent> _poolPlantPoolIndexComponent;

		public BasePlantStrategy(IMapService mapService,
			BrushesData brushesData,
			PlantsData plantsData,
			EcsWorld world) : base(mapService, brushesData)
		{
			_world = world;
			_plantsData = plantsData;
			_plantData = _plantsData.GetPlant(ToolType);

			_poolVoxelTypeCompoenent = world.GetPool<VoxelTypeComponent>();
			_poolVoxelPosition = world.GetPool<VoxelPositionComponent>();
			_poolPlantPoolIndexComponent = world.GetPool<PlantPoolIndexComponent>();
			_poolPlantPosiiton = world.GetPool<PlantPositionComponent>();
			_poolPlantTypeComponent = world.GetPool<PlantTypeComponent>();
		}

		protected abstract int PoolIndex { get; }

		protected override void Use(List<MapNode> entities)
		{
			if (entities.Count == 0)
				return;

			foreach (var mapNode in entities)
			{
				var entity = mapNode.Entity;
				var voxel = _poolVoxelTypeCompoenent.Get(entity).Value;
				foreach (var voxelType in _plantData.VoxelType)
				{
					if (voxel != voxelType)
						continue;

					var position = _poolVoxelPosition.Get(entity).Value;
					var plantPosition = new Vector3(position.x, (int) voxel, position.y);
					var newPlantEntity = _world.NewEntity();
					_poolPlantPosiiton.Add(newPlantEntity).Value = plantPosition;
					_poolPlantTypeComponent.Add(newPlantEntity).Value = _plantData.Type;
					_poolPlantPoolIndexComponent.Add(newPlantEntity).Value = PoolIndex;
					break;
				}
			}
		}
	}
}