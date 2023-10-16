using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct BlockSpawningSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var config = SystemAPI.GetSingleton<GameSettings>();
		int2 mapSize2D = new int2(config.chunkSize * 10, config.chunkSize * 10);

		var ecbSingleton =
			SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
		var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

		var blocksJob = new BlockGenerator
		{
			ECB = ecb.AsParallelWriter(),
			mapSize2D = mapSize2D
		};

		blocksJob.ScheduleParallel();

		state.Enabled = false;
	}
}