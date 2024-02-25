using Db.Brush;
using Game.Services.MapGenerator;
using Services.Map;
using XFlow.EcsLite;

namespace Tools
{
	public class PlainStrategy : BaseVoxelTypeStrategy
	{
		protected override VoxelType VoxelType => VoxelType.Plain;
		public override ToolType ToolType => ToolType.Plain;
		public PlainStrategy(IMapService mapService, BrushesData brushesData, EcsWorld world) : base(mapService, brushesData, world)
		{
		}
	}
}