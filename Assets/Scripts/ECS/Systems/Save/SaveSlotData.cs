using System;
using DefaultNamespace.Utils;
using Game.Services.MapGenerator;
using UnityEngine;

namespace DefaultNamespace.Systems.Save
{
	public class SaveSlotData
	{
		public string Name => $"Save: {_saveData.SaveDateTime}";
		private readonly SaveData _saveData;

		public SaveSlotData(SaveData saveData)
		{
			_saveData = saveData;
		}

		public Texture2D GenerateSavePreview()
		{
			var texture = new Texture2D(WorldUtils.WORLD_SIZE, WorldUtils.WORLD_SIZE);
			foreach (var voxelSaveData in _saveData.VoxelsSaveData)
			{
				var voxelPos = voxelSaveData.VoxelPos;
				var voxelPosX = voxelPos.X + WorldUtils.WORLD_SIZE / 2;
				var voxelPosY = voxelPos.Y + WorldUtils.WORLD_SIZE / 2;
				texture.SetPixel(voxelPosX, voxelPosY, GetColorForVoxel(voxelSaveData.VoxelType));
			}
			
			texture.Apply();
			return texture;
		}

		private Color32 GetColorForVoxel(VoxelType voxelType)
		{
			switch (voxelType)
			{
				case VoxelType.GroundWater:
					return Color.blue;
				case VoxelType.Water:
					return Color.blue;
				case VoxelType.Sand:
					return Color.yellow;
				case VoxelType.Plain:
					return Color.green;
				case VoxelType.Forest:
					return Color.green;
				case VoxelType.Rock:
					return Color.gray;
				default:
					throw new ArgumentOutOfRangeException(nameof(voxelType), voxelType, null);
			}
		}
	}
}