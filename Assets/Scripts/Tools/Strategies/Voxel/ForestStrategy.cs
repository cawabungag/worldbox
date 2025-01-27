using Db.Brush;
using Game.Services.MapGenerator;
using Leopotam.EcsLite;
using Services.Map;

namespace Tools
{
	public class ForestStrategy : BaseVoxelTypeStrategy
	{
		protected override VoxelType VoxelType => VoxelType.Forest;
		public override ToolType ToolType => ToolType.Forest;
		public ForestStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService, brushesData, world)
		{
		}
	}
}