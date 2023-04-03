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
			var splitArray = rawEntities.ToRectangular();

			var nextCoord = new Vector2Int(WorldUtils.CHUNK_SIZE, WorldUtils.CHUNK_SIZE);
			var startCoord = new Vector2Int();

			var length = splitArray.GetLength(1);
			var lengthY = length / WorldUtils.CHUNK_SIZE;
			for (int y = 0; y < lengthY; y++)
			{
				var l = splitArray.GetLength(0);
				var lengthX = l / WorldUtils.CHUNK_SIZE;
				for (int x = 0; x < lengthX; x++)
				{
					var chunkEntity = world.NewEntity();
					var chunk = new Vector4(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
					poolChunkComponent.Add(chunkEntity).Value = chunk;
					Debug.LogError($"Chunk: {chunk}");
					poolChunkViewComponent.Add(chunkEntity).Value = _poolChunks.Spawn();
					var sliceBoard = splitArray.Slice(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);

					for (int i = 0; i < sliceBoard.GetLength(0); i++)
					{
						for (int j = 0; j < sliceBoard.GetLength(1); j++)
						{
							poolChunkEntityComponent.Add(sliceBoard[i, j]).Value = chunkEntity;
						}
					}

					startCoord.x += WorldUtils.CHUNK_SIZE;
					nextCoord.x += WorldUtils.CHUNK_SIZE;
				}

				startCoord.x = 0;
				nextCoord.x = WorldUtils.CHUNK_SIZE;

				startCoord.y += WorldUtils.CHUNK_SIZE;
				nextCoord.y += WorldUtils.CHUNK_SIZE;
			}

			var timestamp2 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			Debug.Log($"PERFOMANCE: GenerateMapSystem: {timestamp2 - timestamp1}");
		}
	}
}