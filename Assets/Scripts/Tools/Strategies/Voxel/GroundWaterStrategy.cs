using Db.Brush;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;

namespace Tools
{
	public class GroundWaterStrategy : BaseVoxelTypeStrategy
	{
		protected override VoxelType VoxelType => VoxelType.GroundWater;
		public override ToolType ToolType => ToolType.GroundWater;
		public GroundWaterStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService, brushesData, world)
		{
		}
	}
}