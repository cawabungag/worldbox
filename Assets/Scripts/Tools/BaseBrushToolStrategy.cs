using Db.Brush;
using Services.Map;
using UnityEngine;

namespace Tools
{
	public abstract class BaseBrushToolStrategy : IUseToolStrategy 
	{
		private readonly IMapService _mapService;
		private readonly BrushesData _brushesData;
		
		public abstract ToolType ToolType { get; }

		public BaseBrushToolStrategy(IMapService mapService, BrushesData brushesData)
		{
			_mapService = mapService;
			_brushesData = brushesData;
		}
		
		public void Use(Vector3 worldTouchPoint, BrushType brushType, int brushSize)
		{
			var brush = _brushesData.GetBrush(brushType, brushSize);
			_mapService.GetVoxelEntities(new Vector2Int((int) worldTouchPoint.x, (int) worldTouchPoint.z), brush);
		}

		protected abstract void Use();
	}
}