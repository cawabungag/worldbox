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
		private const int MINIMAL_VOXEL_TYPE = 1;
		private readonly EcsPool<VoxelTypeComponent> _poolVoxelType;
		public override ToolType ToolType => ToolType.Up;

		public UpBrushToolStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService,
			brushesData)
		{
			_poolVoxelType = world.GetPool<VoxelTypeComponent>();
		}

		protected override void Use(int[] entities)
		{
			if (entities.Length == 0)
				return;

			foreach (var entity in entities)
			{
				if (!_poolVoxelType.Has(entity))
					continue;

				ref var voxelType = ref _poolVoxelType.Get(entity).Value;
				voxelType = (VoxelType) Mathf.Max((int) voxelType--, MINIMAL_VOXEL_TYPE);
			}
		}
	}
}