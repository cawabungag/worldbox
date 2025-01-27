using System;
using Tools;
using UnityEngine;

namespace Db.Tools
{
	[Serializable]
	public class ToolData
	{
		public ToolType ToolType;
		public Sprite Sprite;
		public bool IsBrushTool = false;
		public bool IsSystemTool = false;
		public ToolWindowType WindowType = ToolWindowType.None;
	}
}