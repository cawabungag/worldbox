using System;
using System.Collections.Generic;
using UnityEngine;

namespace Db.Brush
{
	public class BrushesData : ScriptableObject
	{
		[SerializeField]
		private List<BrushData> _brushDatas;

		public BrushData GetBrushData(BrushType brushType, int size)
		{
			foreach (var brushData in _brushDatas)
			{
				if (brushData.BrushType == brushType && brushData.Size == size)
				{
					return brushData;
				}
			}

			throw new InvalidOperationException();
		}

		public Brush GetBrush(BrushType brushType, int size)
		{
			var brush = GetBrushData(brushType, size);
			var sprite = brush.BrushSprite;
			var texture = sprite.texture;
			var pixels = texture.GetPixels();
			var points = new bool[pixels.Length];

			for (var index = 0; index < pixels.Length; index++)
			{
				var pixel = pixels[index];
				points[index] = pixel.r > 0;
			}

			return new Brush(texture.height, texture.width, points);
		}
	}

	[Serializable]
	public class BrushData
	{
		public BrushType BrushType;
		public int Size;
		public Sprite BrushSprite;
	}
}