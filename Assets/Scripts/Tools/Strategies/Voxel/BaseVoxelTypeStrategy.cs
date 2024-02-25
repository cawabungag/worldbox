using System.Collections.Generic;
using Db.Brush;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Services.Map;
using XFlow.EcsLite;

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

		protected override void Use(List<MapNode> entities)
		{
			foreach (var mapNode in entities)
			{
				var entity = mapNode.Entity;
				if (!_poolVoxelType.Has(entity)
					|| !_poolChunkEntity.Has(entity))
					continue;

				var voxelType = _poolVoxelType.Get(entity).Value;
				if (voxelType == VoxelType)
					continue;
				
				_poolVoxelType.GetRef(entity).Value = VoxelType;
				var chunkEntity = _poolChunkEntity.Get(entity).Value;
				_poolNeedUpdateChunk.GetRef(chunkEntity).Value = true;
			}
		}
	}
}