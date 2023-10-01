using Db.Brush;
using Db.Plant;
using DefaultNamespace.Components.Plant;
using Leopotam.EcsLite;
using Services.Map;
using Zenject;

namespace Tools.Plant
{
	public sealed class PalmToolStrategy : BasePlantStrategy
	{
		public PalmToolStrategy(IMapService mapService, BrushesData brushesData, PlantsData plantsData, DiContainer container, EcsWorld world) : base(mapService, brushesData, plantsData, world)
		{
		}

		public override ToolType ToolType => ToolType.Palm;
		protected override int PoolIndex => 0;
	}
}