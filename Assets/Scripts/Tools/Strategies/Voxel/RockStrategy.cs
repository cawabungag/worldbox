using Db.Brush;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;

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