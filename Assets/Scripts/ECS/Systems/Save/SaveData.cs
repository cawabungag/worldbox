using System;
using System.Collections.Generic;
using Game.Services.MapGenerator;
using Plant;

namespace DefaultNamespace.Systems.Save
{
	[Serializable]
	public class SaveData
	{
		public readonly List<VoxelSaveData> VoxelsSaveData = new();
		public readonly List<PlantSaveData> PlantsaveData = new();
		public DateTime SaveDateTime;
	}

	[Serializable]
	public struct VoxelSaveData
	{
		public Vector2IntSerializable VoxelPos { get; }
		public VoxelType VoxelType { get; }

		public VoxelSaveData(Vector2IntSerializable voxelPos, VoxelType voxelType)
		{
			VoxelPos = voxelPos;
			VoxelType = voxelType;
		}
	}
	
	[Serializable]
	public struct PlantSaveData
	{
		public Vector3IntSerializable PlantPos { get; }
		public PlantType PlantType { get; }

		public PlantSaveData(Vector3IntSerializable plantPos, PlantType plantType)
		{
			PlantType = plantType;
			PlantPos = plantPos;
		}
	}
}