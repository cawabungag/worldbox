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

		protected BaseBrushToolStrategy(IMapService mapService, BrushesData brushesData)
		{
			_mapService = mapService;
			_brushesData = brushesData;
		}
		
		public void Use(Vector3 worldTouchPoint, BrushType brushType, int brushSize)
		{
			var brush = _brushesData.GetBrush(brushType, brushSize);
			var entities = _mapService.GetVoxelEntities(new Vector2Int((int) worldTouchPoint.x, (int) worldTouchPoint.z), brush);
			if (entities.Length == 0)
				return;
			
			Debug.LogError($"worldTouchPoint: {worldTouchPoint}");
			Use(entities);
		}

		protected abstract void Use(int[] entities);
	}
}