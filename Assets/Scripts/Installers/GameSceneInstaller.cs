using Game.Services.MapGenerator;
using Game.Services.MapGenerator.Impls;
using Services.Map;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameSceneInstaller : MonoInstaller
	{
		[SerializeField]
		private VoxelRenderCounter _counter;
		public override void InstallBindings()
		{
			Container.Bind<IMapGenerator>().To<MapGenerator>().AsSingle();
			Container.Bind<IMapService>().To<MapService>().AsSingle();
			Container.BindInstance(_counter);
		}
	}
}