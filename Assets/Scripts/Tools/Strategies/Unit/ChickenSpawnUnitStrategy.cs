using DefaultNamespace.Components.Unit;
using DefaultNamespace.Db.Unit;
using Services.Map;
using XFlow.EcsLite;

namespace Tools.Unit
{
	public class ChickenSpawnUnitStrategy : BaseSpawnUnitStrategy
	{
		public override ToolType ToolType => ToolType.Chicken;
		public override int PoolIndex => 0;
		public override UnitType UnitType { get; }

		public ChickenSpawnUnitStrategy(IMapService mapService, UnitsData unitsData, EcsWorld world) : base(mapService, unitsData, world)
		{
		}
	}
}