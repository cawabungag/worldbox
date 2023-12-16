using System;
using DefaultNamespace.Saver;
using UnityEngine;

namespace ECS.Components.Map
{
	[Serializable]
	public struct VoxelPositionComponent : ISerializableComponent<Vector2Int>
	{
		public Vector2Int Value;
		public void Write(Vector2Int value) => Value = value;
		public Vector2Int Read() => Value;
	}
}