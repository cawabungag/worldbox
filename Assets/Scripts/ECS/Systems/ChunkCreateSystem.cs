using DefaultNamespace.Utils;
using ECS.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Systems
{
	public class ChunkCreateSystem : IEcsInitSystem
	{
		private const int WORLD_SIZE = 256;
		private const int CHUNKS_COUNT = 10;

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var entities = world.Filter<VoxelPositionComponent>()
				.Inc<VoxelTypeComponent>()
				.Exc<ChunkComponent>()
				.End();

			var poolChunkComponent = world.GetPool<ChunkComponent>();
			var rawEntities = entities.GetRawEntities();
			var splitArray = ArrayUtils.SplitArray(rawEntities, WORLD_SIZE / 2);
			var jaggedArray = new int[splitArray.GetLength(0)][];
			for (int i = 0; i < splitArray.GetLength(0); i++)
			{
				jaggedArray[i] = new int[splitArray.GetLength(1)];
				for (int j = 0; j < splitArray.GetLength(1); j++)
				{
					jaggedArray[i][j] = splitArray[i, j];
				}
			}

			var worldSideSize = 16;
			var nextCoord = new Vector2Int(worldSideSize, worldSideSize);
			var startCoord = new Vector2Int();

			var lengthY = jaggedArray.Length / worldSideSize;

			for (int y = 0; y < lengthY; y++)
			{
				var lengthX = jaggedArray[y].Length / worldSideSize;
				for (int x = 0; x < lengthX; x++)
				{
					var sliceBoard =
						ArrayUtils.Slice(jaggedArray, startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
					for (int i = 0; i < sliceBoard.GetLength(0); i++)
					{
						for (int k = 0; k < sliceBoard.GetLength(1); k++)
						{
							var entity = sliceBoard[i, k];
							poolChunkComponent.Add(entity).Value =
								new Vector4(startCoord.x, startCoord.y, nextCoord.x, nextCoord.y);
						}
					}
				}

				startCoord.x = 0;
				nextCoord.x = worldSideSize;

				startCoord.y += worldSideSize;
				nextCoord.y += worldSideSize;
			}
		}
	}
}