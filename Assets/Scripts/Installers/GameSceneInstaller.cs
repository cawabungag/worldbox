using DefaultNamespace;
using Game.Services.MapGenerator;
using Game.Services.MapGenerator.Impls;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameSceneInstaller : MonoInstaller
	{
		[SerializeField]
		private WorldRoot _worldRoot;
		public override void InstallBindings()
		{
			Container.Bind<IMapGenerator>().To<MapGenerator>().AsSingle();
			Container.BindInstance(_worldRoot);
		}
	}
}