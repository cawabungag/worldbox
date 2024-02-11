using UnityEngine;

namespace Tools.Unit
{
	public class ChickenSpawnUnitStrategy : BaseSpawnUnitStrategy
	{
		public override ToolType ToolType => ToolType.Chicken;
		public override int PoolIndex => 0;

		public override void SingleCellUse(Vector3 worldTouchPoint)
		{
			throw new System.NotImplementedException();
		}
	}
}