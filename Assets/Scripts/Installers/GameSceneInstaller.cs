using System;
using Core.Camera;
using Core.Camera.Impl;
using Game.Services.MapGenerator;
using Game.Services.MapGenerator.Impls;
using Services.Map;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameSceneInstaller : MonoInstaller
	{
		[SerializeField]
		private InputView _inputView;

		[SerializeField]
		private SceneCamera _sceneCamera;
		
		public override void InstallBindings()
		{
			Container.BindInstance(_inputView);
			Container.Bind<ISceneCamera>().To<SceneCamera>().FromInstance(_sceneCamera);
			Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<InputController>().AsSingle().NonLazy();
			Container.Bind<IMapGenerator>().To<MapGenerator>().AsSingle();
			Container.Bind<IMapService>().To<MapService>().AsSingle();
		}
	}
}