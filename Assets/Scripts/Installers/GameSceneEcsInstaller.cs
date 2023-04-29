using DefaultNamespace;
using DefaultNamespace.Chunk;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Utils;
using ECS.Systems;
using Leopotam.EcsLite;
using Tools;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameSceneEcsInstaller : MonoInstaller
	{
		[SerializeField]
		private ChunkView _chunkView;

		public override void InstallBindings()
		{
			var config = new EcsWorld.Config();
			config.Entities = WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE + WorldUtils.CHUNK_SIZE;
			config.Filters = WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE + WorldUtils.CHUNK_SIZE;
			config.Pools = WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE + WorldUtils.CHUNK_SIZE;
			var ecsWorld = new EcsWorld(config);
			Container.BindInstance(ecsWorld);

			var inputEcsWorld = new EcsWorld();
			Container.BindInstance(inputEcsWorld).WithId(WorldUtils.INPUT_WORLD_NAME);
			var newEntity = inputEcsWorld.NewEntity();
			inputEcsWorld.GetPool<InputToolComponent>().Add(newEntity).Value = ToolType.Up;


			Container.BindInterfacesAndSelfTo<EcsBootstrap>().AsSingle().NonLazy();
			Container.Bind<IEcsSystem>().To<GenerateMapSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<ChunkCreateSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<VoxelRenderSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<UpdateChunksSystem>().AsSingle();

			Container.BindMemoryPool<ChunkView, ChunkView.Pool>()
				.WithInitialSize(16)
				.FromComponentInNewPrefab(_chunkView)
				.UnderTransformGroup("Chunks");
		}
	}
}