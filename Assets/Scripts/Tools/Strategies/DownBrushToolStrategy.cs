using System.Collections.Generic;
using Db.Brush;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;

namespace Tools
{
	public class DownBrushToolStrategy : BaseBrushToolStrategy
	{
		private const int MINIMUM_VOXEL_TYPE = 1;
		private readonly EcsPool<VoxelTypeComponent> _poolVoxelType;
		private readonly EcsPool<ChunkEntityComponent> _poolChunkEntity;
		private readonly EcsPool<NeedUpdateChunkComponent> _poolNeedUpdateChunk;
		public override ToolType ToolType => ToolType.Down;

		public DownBrushToolStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService,
			brushesData)
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
				var newVoxelType = voxelType - 1;
				newVoxelType = (VoxelType) Mathf.Max((int) newVoxelType, MINIMUM_VOXEL_TYPE);
				var chunkEntity = _poolChunkEntity.Get(entity).Value;
				_poolVoxelType.Get(entity).Value = newVoxelType;
				_poolNeedUpdateChunk.Get(chunkEntity).Value = true;
			}
		}
	}
}