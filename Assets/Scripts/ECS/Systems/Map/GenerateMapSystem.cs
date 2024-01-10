using DefaultNamespace.Systems.Save;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class GenerateMapSystem : IEcsInitSystem
	{
		private readonly IMapGenerator _mapGenerator;
		private readonly ISaveModel _saveModel;

		public GenerateMapSystem(IMapGenerator mapGenerator, ISaveModel saveModel)
		{
			_mapGenerator = mapGenerator;
			_saveModel = saveModel;
		}

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var voxelPositionPool = world.GetPool<VoxelPositionComponent>();
			var voxelTypePool = world.GetPool<VoxelTypeComponent>();
			
			var halfWidth = WorldUtils.WORLD_SIZE / 2;
			var halfHeight = WorldUtils.WORLD_SIZE / 2;
			var mapGraph = new GridGraph<MapNode>(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);

			int mapGraphEntity;
			if (_saveModel.LastSave != null)
			{
				var voxelsSaveData = _saveModel.LastSave.VoxelsSaveData;
				foreach (var componentData in voxelsSaveData)
				{
					var voxelEntity = world.NewEntity();
					var voxelPos = componentData.VoxelPos;
					var cellPosiiton = new Vector2Int(voxelPos.X, voxelPos.Y);
					voxelPositionPool.Add(voxelEntity).Value = cellPosiiton;
					voxelTypePool.Add(voxelEntity).Value = componentData.VoxelType;
					var mapNode = new MapNode(cellPosiiton, voxelEntity);
					mapGraph.SetEntity(cellPosiiton.x + halfWidth, cellPosiiton.y + halfHeight, mapNode);
					mapGraphEntity = world.NewEntity();
					world.GetPool<MapGraphComponent>().Add(mapGraphEntity).Value = mapGraph;
				}
				
				return;
			}

			var voxels = _mapGenerator.GenerateGround(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);

			for (var i = 0; i < WorldUtils.WORLD_SIZE; i++)
			{
				for (var j = 0; j < WorldUtils.WORLD_SIZE; j++)
				{
					var x = j - halfWidth;
					var z = i - halfHeight;
					var tileType = voxels[WorldUtils.WORLD_SIZE * i + j];

					var voxelEntity = world.NewEntity();
					var cellPosiiton = new Vector2Int(x, z);
					voxelPositionPool.Add(voxelEntity).Value = cellPosiiton;
					voxelTypePool.Add(voxelEntity).Value = tileType;
					var mapNode = new MapNode(cellPosiiton, voxelEntity);
					mapGraph.SetEntity(j, i, mapNode);
				}
			}

			mapGraphEntity = world.NewEntity();
			world.GetPool<MapGraphComponent>().Add(mapGraphEntity).Value = mapGraph;
		}
	}
}