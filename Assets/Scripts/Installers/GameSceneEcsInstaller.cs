using DefaultNamespace;
using ECS.Systems;
using Leopotam.EcsLite;
using Zenject;

namespace Installers
{
	public class GameSceneEcsInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<EcsBootstrap>().AsSingle().NonLazy();
			Container.Bind<IEcsSystem>().To<GenerateMapSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<VoxelRenderSystem>().AsSingle();
		}
	}
}