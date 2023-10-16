using System.Collections.Generic;
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

		public void Use(Vector3 worldTouchPoint,
			BrushType brushType,
			int brushSize,
			bool isBrushTool)
		{
			//TODO Refactoring convert posiiton
			var inputPoint = new Vector2Int((int) worldTouchPoint.x, (int) worldTouchPoint.y);

			if (brushType == BrushType.None || brushSize == 0)
			{
				var entity = _mapService.GetVoxelEntity(inputPoint);
				if (entity == -1)
					return;

				Use(new List<MapNode> { new(inputPoint, entity) });
				return;
			}

			var brush = _brushesData.GetBrush(brushType, brushSize);
			var entities = _mapService.GetVoxelEntities(inputPoint, brush);
			if (entities.Count == 0)
				return;

			Use(entities);
		}

		protected abstract void Use(List<MapNode> entities);
	}
}