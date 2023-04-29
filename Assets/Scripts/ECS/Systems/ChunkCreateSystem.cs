using System.Collections.Generic;
using DefaultNamespace.Chunk;
using DefaultNamespace.Utils;
using ECS.Components.Map;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class ChunkCreateSystem : IEcsInitSystem
	{
		private readonly ChunkView.Pool _poolChunks;
		private readonly List<int> _voxelEntitiesBuffer = new();

		public ChunkCreateSystem(ChunkView.Pool poolChunks)
		{
			_poolChunks = poolChunks;
		}

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var entities = world.Filter<VoxelPositionComponent>()
				.Inc<VoxelTypeComponent>()
				.Exc<ChunkComponent>()
				.End();

			var poolChunkComponent = world.GetPool<ChunkComponent>();
			var poolChunkViewComponent = world.GetPool<ChunkViewCompoenent>();
			var poolChunkEntityComponent = world.GetPool<ChunkEntityComponent>();
			var poolNeedUpdateChunk = world.GetPool<NeedUpdateChunkComponent>();
			var poolVoxelsInChunk = world.GetPool<VoxelsInChunkComponent>();
			var rawEntities = entities.GetRawEntities();
			var splitArray = rawEntities.ToRectangular();

			var nextCoord = new Vector2Int(WorldUtils.CHUNK_SIZE, WorldUtils.CHUNK_SIZE);
			var startCoord = new Vector2Int();

			var lengthFirstDimension = splitArray.GetLength(1);
			var lengthY = lengthFirstDimension / WorldUtils.CHUNK_SIZE;
			for (int y = 0; y < lengthY; y++)
			{
				var lengthSecondDimension = splitArray.GetLength(0);
				var lengthX = lengthSecondDimension / WorldUtils.CHUNK_SIZE;
				for (int x = 0; x < lengthX; x++)
				{
					_voxelEntitiesBuffer.Clear();
					var chunkEntity = world.NewEntity();
					var chunk = new Vector4(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
					poolChunkComponent.Add(chunkEntity).Value = chunk;
					poolChunkViewComponent.Add(chunkEntity).Value = _poolChunks.Spawn();
					var sliceBoard = splitArray.Slice(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
					poolNeedUpdateChunk.Add(chunkEntity).Value = false;

					for (int i = 0; i < sliceBoard.GetLength(0); i++)
					{
						for (int j = 0; j < sliceBoard.GetLength(1); j++)
						{
							var voxelEntity = sliceBoard[i, j];
							_voxelEntitiesBuffer.Add(voxelEntity);
							poolChunkEntityComponent.Add(voxelEntity).Value = chunkEntity;
						}
					}

					poolVoxelsInChunk.Add(chunkEntity).Value = _voxelEntitiesBuffer.ToArray();
					startCoord.x += WorldUtils.CHUNK_SIZE;
					nextCoord.x += WorldUtils.CHUNK_SIZE;
				}

				startCoord.x = 0;
				nextCoord.x = WorldUtils.CHUNK_SIZE;

				startCoord.y += WorldUtils.CHUNK_SIZE;
				nextCoord.y += WorldUtils.CHUNK_SIZE;
			}
		}
	}
}