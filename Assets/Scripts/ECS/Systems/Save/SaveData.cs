using System;
using System.Collections.Generic;
using Game.Services.MapGenerator;
using ModestTree;
using Plant;
using UnityEngine;

namespace DefaultNamespace.Systems.Save
{
	[Serializable]
	public class SaveData
	{
		public readonly Dictionary<Vector2IntSerializable, VoxelType> VoxelsSaveData = new();
		public readonly Dictionary<Vector3IntSerializable, PlantType> PlantsaveData = new();
		public bool IsEmpty => VoxelsSaveData.IsEmpty() && PlantsaveData.IsEmpty();
	}

	[Serializable]
	public struct Vector2IntSerializable
	{
		public int X;
		public int Y;

		public Vector2IntSerializable(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
	
	[Serializable]
	public struct Vector3IntSerializable
	{
		public int X;
		public int Y;
		public int Z;

		public Vector3IntSerializable(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
	
	public static class SaveUtils
	{
		public static Vector2IntSerializable AsSerializable(this Vector2Int value)
		{
			return new Vector2IntSerializable(value.x, value.y);
		}
		
		public static Vector3IntSerializable AsSerializable(this Vector3Int value)
		{
			return new Vector3IntSerializable(value.x, value.y, value.z);
		}
	}
}