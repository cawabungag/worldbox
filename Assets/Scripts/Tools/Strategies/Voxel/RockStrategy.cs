using Db.Brush;
using Game.Services.MapGenerator;
using Services.Map;
using XFlow.EcsLite;

namespace Tools
{
	public class RockStrategy : BaseVoxelTypeStrategy
	{
		protected override VoxelType VoxelType => VoxelType.Rock;
		public override ToolType ToolType => ToolType.Rock;

		public RockStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService,
			brushesData, world)
		{
		}
	}
}