using System;
using System.Collections.Generic;
using System.Linq;
using Db.Brush;
using ECS.Components.Map;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;

namespace Tools
{
	public class UpBrushToolStrategy : BaseBrushToolStrategy
	{
		private readonly EcsPool<VoxelTypeComponent> _poolVoxelType;
		private readonly EcsPool<ChunkEntityComponent> _poolChunkEntity;
		private readonly EcsPool<NeedUpdateChunkComponent> _poolNeedUpdateChunk;
		public override ToolType ToolType => ToolType.Up;

		public UpBrushToolStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService,
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
				var newVoxelType = voxelType + 1;
				var maxVoxelType = Enum.GetValues(typeof(VoxelType)).Cast<byte>().Max() + 1;
				newVoxelType = (VoxelType) Mathf.Min((int) newVoxelType, maxVoxelType);
				var chunkEntity = _poolChunkEntity.Get(entity).Value;
				_poolVoxelType.Get(entity).Value = newVoxelType;
				_poolNeedUpdateChunk.Get(chunkEntity).Value = true;
			}
		}
	}
}