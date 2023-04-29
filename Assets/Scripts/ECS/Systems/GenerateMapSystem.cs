using DefaultNamespace.Utils;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;

namespace ECS.Systems
{
	public class GenerateMapSystem : IEcsInitSystem
	{
		private readonly IMapGenerator _mapGenerator;
		private readonly IMapService _mapService;

		public GenerateMapSystem(IMapGenerator mapGenerator, IMapService mapService)
		{
			_mapGenerator = mapGenerator;
			_mapService = mapService;
		}

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var voxelPositionPool = world.GetPool<VoxelPositionComponent>();
			var voxelTypePool = world.GetPool<VoxelTypeComponent>();

			var voxels = _mapGenerator.GenerateGround(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);
			var halfWidth = WorldUtils.WORLD_SIZE / 2;
			var halfHeight = WorldUtils.WORLD_SIZE / 2;

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
				}
			}
		}
	}
}