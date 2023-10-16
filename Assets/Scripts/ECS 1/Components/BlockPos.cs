using Unity.Entities;
using Unity.Mathematics;

namespace DefaultNamespace.ECS_1.Components
{
	public struct BlockPos : IComponentData
	{
		public float3 spawnPos;
	}
}