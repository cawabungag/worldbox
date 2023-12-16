using System;
using DefaultNamespace.Saver;
using UnityEngine;

namespace DefaultNamespace.Components.Plant
{
	[Serializable]
	public struct PlantPositionComponent : ISerializableComponent<Vector3Int>
	{
		public Vector3Int Value;
		public void Write(Vector3Int value) => Value = value;
		public Vector3Int Read() => Value;
	}
}