using System.Collections.Generic;
using Plant;
using Tools;
using UnityEngine;

namespace Db.Plant
{
	[CreateAssetMenu(fileName = "PlantsData", menuName = "Data/PlantsData")]
	public class PlantsData : ScriptableObject
	{
		[SerializeField]
		private List<PlantData> _plantsData;

		public List<PlantData> GetPlantsData() => _plantsData;
		public PlantData GetPlant(PlantType plantType) => _plantsData.Find(x => x.Type == plantType);
		public PlantData GetPlant(ToolType toolType) => _plantsData.Find(x => x.ToolType == toolType);
	}
}