using Db.Brush;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;

namespace Tools
{
	public class WaterStrategy : BaseVoxelTypeStrategy
	{
		protected override VoxelType VoxelType => VoxelType.Water;
		public override ToolType ToolType => ToolType.Water;
		public WaterStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService, brushesData, world)
		{
		}
	}
}