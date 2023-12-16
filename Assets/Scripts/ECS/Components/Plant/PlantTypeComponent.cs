using System;
using DefaultNamespace.Saver;
using Plant;

namespace DefaultNamespace.Components.Plant
{
	[Serializable]
	public struct PlantTypeComponent : ISerializableComponent<PlantType>
	{
		public PlantType Value;
		public void Write(PlantType value) => Value = value;
		public PlantType Read() => Value;
	}
}