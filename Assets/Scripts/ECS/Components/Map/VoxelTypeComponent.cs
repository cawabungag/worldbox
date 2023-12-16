using System;
using DefaultNamespace.Saver;
using Game.Services.MapGenerator;

namespace ECS.Components.Map
{
	[Serializable]
	public struct VoxelTypeComponent : ISerializableComponent<VoxelType>
	{
		public VoxelType Value;
		public void Write(VoxelType value) => Value = value;
		public VoxelType Read() => Value;
	}
}