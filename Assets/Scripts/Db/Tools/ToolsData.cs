using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Db.Tools
{
	[CreateAssetMenu(fileName = "ToolsData", menuName = "Data/ToolsData")]
	public class ToolsData : ScriptableObject
	{
		[SerializeField]
		private List<ToolData> _toolDatas;

		public List<ToolData> GetTools() => _toolDatas;
		public ToolData GetTool(ToolType toolType) => _toolDatas.Find(x => x.ToolType == toolType);
	}
}