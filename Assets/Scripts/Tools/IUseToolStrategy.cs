using Db.Brush;
using UnityEngine;

namespace Tools
{
	public interface IUseToolStrategy
	{
		ToolType ToolType { get; }
		void UseBrush(Vector3 worldTouchPoint, BrushType brushType, int brushSize);
		void SingleCellUse(Vector3 worldTouchPoint);
	}
}