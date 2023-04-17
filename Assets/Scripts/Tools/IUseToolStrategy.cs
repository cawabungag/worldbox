using Db.Brush;
using UnityEngine;

namespace Tools
{
	public interface IUseToolStrategy
	{
		ToolType ToolType { get; }
		void Use(Vector3 worldTouchPoint, BrushType brushType, int brushSize);
	}
}