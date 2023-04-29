using System;
using Core.Camera;
using Core.Camera.Impl;
using Db.Brush;
using Db.Tools;
using Game.Services.MapGenerator;
using Game.Services.MapGenerator.Impls;
using Services.Map;
using Tools;
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

		[SerializeField]
		private BrushesData _brushData;

		[SerializeField]
		private ToolsData _toolsData;

		public override void InstallBindings()
		{
			Container.BindInstance(_inputView);
			Container.BindInstance(_brushData);
			Container.BindInstance(_toolsData);
			Container.Bind<ISceneCamera>().To<SceneCamera>().FromInstance(_sceneCamera);
			Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(ILateTickable)).To<InputController>()
				.AsSingle().NonLazy();
			Container.Bind<IMapGenerator>().To<MapGenerator>().AsSingle();
			Container.Bind<IMapService>().To<MapService>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<UpBrushToolStrategy>().AsSingle();
		}
	}
}