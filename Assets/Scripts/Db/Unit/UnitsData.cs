using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace DefaultNamespace.Db.Unit
{
	[CreateAssetMenu(fileName = "UnitsData", menuName = "Data/UnitsData")]
	public class UnitsData : ScriptableObject
	{
		[SerializeField]
		private List<UnitData> _unitDatas;

		public List<UnitData> GetUnits() => _unitDatas;
		public UnitData GetUnit(ToolType toolType) => _unitDatas.Find(x => x.ToolType == toolType);
	}

	[Serializable]
	public class UnitData
	{
		public ToolType ToolType;
	}
}