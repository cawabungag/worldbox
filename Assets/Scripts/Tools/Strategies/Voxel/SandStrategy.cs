using Db.Brush;
using Game.Services.MapGenerator;
using Services.Map;
using XFlow.EcsLite;

namespace Tools
{
	public class SandStrategy : BaseVoxelTypeStrategy
	{
		protected override VoxelType VoxelType => VoxelType.Sand;
		public override ToolType ToolType => ToolType.Sand;
		public SandStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService, brushesData, world)
		{
		}
	}
}