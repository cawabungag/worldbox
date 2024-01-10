using System;
using Core.Camera;
using Core.Camera.Impl;
using Db.Brush;
using Db.Tools;
using DefaultNamespace.Systems.Save;
using Game.Services.MapGenerator;
using Game.Services.MapGenerator.Impls;
using Services.Map;
using Tools;
using Tools.Plant;
using Tools.Strategies.Plant;
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
			Container.Bind(typeof(ISaveModel), typeof(IInitializable)).To<SaveModel>().AsSingle();

			Container.Bind<IUseToolStrategy>().To<UpBrushToolStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<DownBrushToolStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<ForestStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<GroundWaterStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<PlainStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<RockStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<SandStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<WaterStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<PalmToolStrategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<TreeToolStategy>().AsSingle();
			Container.Bind<IUseToolStrategy>().To<CactusToolStrategy>().AsSingle();
		}
	}
}