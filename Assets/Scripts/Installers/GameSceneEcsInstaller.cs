using System.Collections.Generic;
using Db.Plant;
using DefaultNamespace;
using DefaultNamespace.Chunk;
using DefaultNamespace.Components.Input;
using DefaultNamespace.Systems.Plant;
using DefaultNamespace.Systems.Save;
using DefaultNamespace.Utils;
using ECS.Systems;
using Leopotam.EcsLite;
using Plant;
using Tools;
using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameSceneEcsInstaller : MonoInstaller
	{
		[SerializeField]
		private ChunkView _chunkView;

		[SerializeField]
		private PlantsData _plantsData;

		public override void InstallBindings()
		{
			Application.targetFrameRate = 60;
			
			var config = new EcsWorld.Config
			{
				Entities = WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE + WorldUtils.CHUNK_SIZE,
				Filters = WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE + WorldUtils.CHUNK_SIZE,
				Pools = WorldUtils.WORLD_SIZE * WorldUtils.WORLD_SIZE + WorldUtils.CHUNK_SIZE
			};
			
			var ecsWorld = new EcsWorld(config);
			Container.BindInstance(ecsWorld);

			Container.BindInstance(_plantsData);

			var inputEcsWorld = new EcsWorld();
			Container.BindInstance(inputEcsWorld).WithId(WorldUtils.INPUT_WORLD_NAME);
			var newEntity = inputEcsWorld.NewEntity();
			
			//TODO Move to Init system
			inputEcsWorld.GetPool<InputToolComponent>().Add(newEntity).Value = ToolType.None;
			inputEcsWorld.GetPool<InputBrushSizeComponent>().Add(newEntity).Value = InputUtils.DEFAULT_BRUSH_SIZE;
			inputEcsWorld.GetPool<InputBrushTypeComponent>().Add(newEntity).Value = InputUtils.DEFAULT_BRUSH_TYPE;
			inputEcsWorld.GetPool<InputIsBrushToolComponent>().Add(newEntity).Value = false;

			Container.BindInterfacesAndSelfTo<EcsBootstrap>().AsSingle().NonLazy();
			Container.Bind<IEcsSystem>().To<GenerateMapSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<GenerateWeatherSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<ChunkCreateSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<VoxelRenderSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<UpdateChunksSystem>().AsSingle();
			
			Container.Bind<IEcsSystem>().To<GeneratePlantSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<CreatePlantSystem>().AsSingle();
			Container.Bind<IEcsSystem>().To<SaveWorldSystem>().AsSingle();

			Container.BindMemoryPool<ChunkView, ChunkView.Pool>()
				.WithInitialSize(16)
				.FromComponentInNewPrefab(_chunkView)
				.UnderTransformGroup("Chunks");

			var plantsData = _plantsData.GetPlantsData();
			if (plantsData.Count != 0) 
				SetupPlantsPool(plantsData);
		}

		private void SetupPlantsPool(List<PlantData> plantsData)
		{
			foreach (var plantData in plantsData)
			{
				Container.BindMemoryPool<PlantView, PlantView.Pool>()
					.WithInitialSize(plantData.ChunkSize)
					.FromComponentInNewPrefabResource(plantData.PrefabPath)
					.UnderTransformGroup(plantData.PrefabPath)
					.AsCached()
					.WithConcreteId(plantData.PrefabPath);
			}
		}
	}
}