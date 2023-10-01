using System;
using Game.Services.MapGenerator;
using Plant;
using Tools;

namespace Db.Plant
{
	[Serializable]
	public class PlantData
	{
		public PlantType Type;
		public string PrefabPath;
		public int ChunkSize;
		public ToolType ToolType;
		public VoxelType[] VoxelType;
	}
}