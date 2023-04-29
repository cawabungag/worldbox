using DefaultNamespace;
using DefaultNamespace.Chunk;
using DefaultNamespace.Utils;
using ECS.Systems;
using Leopotam.EcsLite;
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
			var ecsWorld = new EcsWorld();
			Container.BindInstance(ecsWorld);

			var inputEcsWorld = new EcsWorld();
			Container.BindInstance(inputEcsWorld).WithId(WorldUtils.INPUT_WORLD_NAME);
			inputEcsWorld.NewEntity();
			
			Container.BindInterfacesTo<EcsBootstrap>().AsSingle().NonLazy();
			Container.Bind<IEcsSystem>().To<GenerateMapSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<ChunkCreateSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<VoxelRenderSystem>().AsSingle();
			
			Container.BindMemoryPool<ChunkView, ChunkView.Pool>()
				.WithInitialSize(16)
				.FromComponentInNewPrefab(_chunkView)
				.UnderTransformGroup("Chunks");
			
		}
	}
}