using Db.Brush;
using Db.Plant;
using DefaultNamespace.Components.Plant;
using Leopotam.EcsLite;
using Services.Map;
using Zenject;

namespace Tools.Plant
{
	public class TreeToolStategy : BasePlantStrategy
	{
		public TreeToolStategy(IMapService mapService, BrushesData brushesData, PlantsData plantsData, DiContainer container, EcsWorld world) : base(mapService, brushesData, plantsData, world)
		{
		}

		public override ToolType ToolType => ToolType.Tree;
		protected override int PoolIndex => 1;
	}
}