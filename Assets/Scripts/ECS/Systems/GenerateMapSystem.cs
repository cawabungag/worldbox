using System;
using ECS.Components;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class GenerateMapSystem : IEcsInitSystem
	{
		private const int WORLD_SIZE = 256;
		private readonly IMapGenerator _mapGenerator;

		public GenerateMapSystem(IMapGenerator mapGenerator) 
			=> _mapGenerator = mapGenerator;

		public void Init(IEcsSystems systems)
		{
			var timestamp1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			var world = systems.GetWorld();
			var voxelPositionPool = world.GetPool<VoxelPositionComponent>();
			var voxelTypePool = world.GetPool<VoxelTypeComponent>();
			
			var voxels = _mapGenerator.GenerateGround(WORLD_SIZE, WORLD_SIZE);
			var halfWidth = WORLD_SIZE / 2;
			var halfHeight = WORLD_SIZE / 2;

			for (var i = 0; i < WORLD_SIZE; i++)
			{
				for (var j = 0; j < WORLD_SIZE; j++)
				{
					var x = j - halfWidth;
					var z = i - halfHeight;
					var tileType = voxels[WORLD_SIZE * i + j];

					for (int y = 0; y < (byte) tileType; y++)
					{
						var voxelEntity = world.NewEntity();
						voxelPositionPool.Add(voxelEntity).Value = new Vector3Int(x, y, z);
						voxelTypePool.Add(voxelEntity).Value = tileType;
					}
				}
			}
			
			var timestamp2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			Debug.Log($"GenerateMapSystem: {timestamp2 - timestamp1}");
		}
	}
}