using DefaultNamespace;
using DefaultNamespace.Chunk;
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
			Container.BindInterfacesTo<EcsBootstrap>().AsSingle().NonLazy();
			Container.Bind<IEcsSystem>().To<GenerateMapSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<VoxelRenderSystem>().AsSingle();
			
			Container.BindMemoryPool<ChunkView, ChunkView.Pool>()
				.WithInitialSize(10)
				.FromComponentInNewPrefab(_chunkView)
				.UnderTransformGroup("Chunks");
		}
	}
}