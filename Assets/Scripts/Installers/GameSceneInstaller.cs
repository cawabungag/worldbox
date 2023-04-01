using Game.Services.MapGenerator;
using Game.Services.MapGenerator.Impls;
using Zenject;

namespace Installers
{
	public class GameSceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IMapGenerator>().To<MapGenerator>().AsSingle();
		}
	}
}