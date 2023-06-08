using System.Collections.Generic;
using Db.Brush;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;

namespace Tools
{
	public abstract class BaseVoxelTypeStrategy : BaseBrushToolStrategy
	{
		protected abstract VoxelType VoxelType { get; }
		private readonly EcsPool<VoxelTypeComponent> _poolVoxelType;
		private readonly EcsPool<ChunkEntityComponent> _poolChunkEntity;
		private readonly EcsPool<NeedUpdateChunkComponent> _poolNeedUpdateChunk;

		protected BaseVoxelTypeStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world)
			: base(mapService, brushesData)
		{
			_poolVoxelType = world.GetPool<VoxelTypeComponent>();
			_poolChunkEntity = world.GetPool<ChunkEntityComponent>();
			_poolNeedUpdateChunk = world.GetPool<NeedUpdateChunkComponent>();
		}

		protected override void Use(List<int> entities)
		{
			foreach (var entity in entities)
			{
				if (!_poolVoxelType.Has(entity)
					|| !_poolChunkEntity.Has(entity))
					continue;

				var voxelType = _poolVoxelType.Get(entity).Value;
				if (voxelType == VoxelType)
					continue;
				
				_poolVoxelType.Get(entity).Value = VoxelType;
				var chunkEntity = _poolChunkEntity.Get(entity).Value;
				_poolNeedUpdateChunk.Get(chunkEntity).Value = true;
			}
		}
	}
}