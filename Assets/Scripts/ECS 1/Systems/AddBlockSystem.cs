using Unity.Entities;
using Unity.Transforms;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
[RequireMatchingQueriesForUpdate]
partial class AddBlockSystem : SystemBase
{
	private BeginSimulationEntityCommandBufferSystem m_BeginSimECBSystem;
	//public static Entity spawnBlock;

	protected override void OnCreate()
	{
		m_BeginSimECBSystem =
			World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var ecb = m_BeginSimECBSystem.CreateCommandBuffer(); //.AsParallelWriter();
		var blockType = SystemAPI.GetSingleton<BlockType>();


		Entities
			.WithAll<AddBlock>()
			.WithNone<RemoveBlock>()
			.ForEach((Entity entity, ref AddBlock addBlock) =>
			{
				var spawnBlock = blockType.defaultPrefab;

				if (addBlock.spawnType == 0)
				{
					spawnBlock = blockType.sixSidedPrefab;
				}
				else if (addBlock.spawnType == 2)
				{
					spawnBlock = blockType.defaultAlphaPrefab;
				}

				//Instantate a prefab block
				var e = ecb.Instantiate(spawnBlock);
				//ecb.SetComponent(e, new Translation {Value = addBlock.spawnPos});
				ecb.SetComponent(e, new LocalTransform {Position = addBlock.spawnPos, Scale = 1});
				ecb.SetComponent(e, new BlockID {blockID = addBlock.spawnMat});

				//Add remove block tag
				ecb.AddComponent(entity, new RemoveBlock());
			}).Schedule(); //Parallel();

		m_BeginSimECBSystem.AddJobHandleForProducer(Dependency);
	}
}