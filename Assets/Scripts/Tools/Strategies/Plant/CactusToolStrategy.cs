using Db.Brush;
using Db.Plant;
using DefaultNamespace.Components.Plant;
using Leopotam.EcsLite;
using Services.Map;
using Zenject;

namespace Tools.Strategies.Plant
{
	public class CactusToolStrategy : BasePlantStrategy
	{
		public CactusToolStrategy(IMapService mapService, BrushesData brushesData, PlantsData plantsData, DiContainer container, EcsWorld world) : base(mapService, brushesData, plantsData, world)
		{
		}

		public override ToolType ToolType => ToolType.Cactus;
		protected override int PoolIndex => 2;
	}
}