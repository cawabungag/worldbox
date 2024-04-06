using DefaultNamespace.Components.Unit;
using DefaultNamespace.Db.Unit;
using Leopotam.EcsLite;
using Services.Map;
using UnityEngine;

namespace Tools.Unit
{
	public class ChickenSpawnUnitStrategy : BaseSpawnUnitStrategy
	{
		public override ToolType ToolType => ToolType.Chicken;
		public override int PoolIndex => 0;
		public override UnitType UnitType { get; }

		// public override void SingleCellUse(Vector3 worldTouchPoint)
		// {
			// throw new System.NotImplementedException();
		// }

		public ChickenSpawnUnitStrategy(IMapService mapService, UnitsData unitsData, EcsWorld world) : base(mapService, unitsData, world)
		{
		}
	}
}