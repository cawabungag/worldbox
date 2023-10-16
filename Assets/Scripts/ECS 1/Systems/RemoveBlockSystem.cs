using DefaultNamespace.ECS_1.Components;
using Unity.Mathematics;
using Unity.Entities;

[UpdateAfter(typeof(AddBlockSystem))]
partial class RemoveBlockSystem : SystemBase
{
	private BeginSimulationEntityCommandBufferSystem m_BeginSimECBSystem;

	protected override void OnCreate()
	{
		m_BeginSimECBSystem =
			World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var ecb = m_BeginSimECBSystem.CreateCommandBuffer();
		var posTemp = new float3(0, 0, 0);

		Entities
			.WithAll<RemoveBlock>()
			.ForEach((Entity entity, in BlockPos blockPos) =>
			{
				var blockPosSpawnPos = blockPos.spawnPos;
				posTemp = new float3(blockPosSpawnPos.x, blockPosSpawnPos.y + 1,
					blockPosSpawnPos.z);
				ecb.DestroyEntity(entity);
			})
			.WithoutBurst()
			.Run();

		//ecb.Dispose();
		m_BeginSimECBSystem.AddJobHandleForProducer(Dependency);
	}
}