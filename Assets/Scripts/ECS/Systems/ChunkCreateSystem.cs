using System;
using DefaultNamespace.Chunk;
using DefaultNamespace.Utils;
using ECS.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class ChunkCreateSystem : IEcsInitSystem
	{
		private readonly ChunkView.Pool _poolChunks;

		public ChunkCreateSystem(ChunkView.Pool poolChunks)
		{
			_poolChunks = poolChunks;
		}
		
		public void Init(IEcsSystems systems)
		{
			var timestamp1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

			var world = systems.GetWorld();
			var entities = world.Filter<VoxelPositionComponent>()
				.Inc<VoxelTypeComponent>()
				.Exc<ChunkComponent>()
				.End();

			var poolChunkComponent = world.GetPool<ChunkComponent>();
			var poolChunkViewComponent = world.GetPool<ChunkViewCompoenent>();
			var poolChunkEntityComponent = world.GetPool<ChunkEntityComponent>();
			var rawEntities = entities.GetRawEntities();
			
			var worldSideSize = (int) Mathf.Sqrt(WorldUtils.WORLD_SIZE);
			var splitArray = rawEntities.ToRectangular(WorldUtils.WORLD_SIZE);
			
			var nextCoord = new Vector2Int(worldSideSize, worldSideSize);
			var startCoord = new Vector2Int();

			for (int y = 0; y < worldSideSize; y++)
			{
				var chunkEntity = world.NewEntity();
				poolChunkComponent.Add(chunkEntity).Value = new Vector4(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
				poolChunkViewComponent.Add(chunkEntity).Value = _poolChunks.Spawn();
				
				for (int x = 0; x < worldSideSize; x++)
				{
					var sliceBoard = splitArray.Slice(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
					for (int i = 0; i < sliceBoard.GetLength(0); i++)
					{
						for (int k = 0; k < sliceBoard.GetLength(1); k++)
						{
							var entity = sliceBoard[i, k];

							//todo: Refactoring
							if (!poolChunkEntityComponent.Has(entity))
							{
								poolChunkEntityComponent.Add(entity).Value = chunkEntity;
							}
						}
					}
				}
				
				startCoord.x = 0;
				nextCoord.x = worldSideSize;

				startCoord.y += worldSideSize;
				nextCoord.y += worldSideSize;
			}
			
			var timestamp2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			Debug.Log($"GenerateMapSystem: {timestamp2 - timestamp1}");
		}
	}
}