using System.Collections.Generic;
using DefaultNamespace.Components.Plant;
using Leopotam.EcsLite;
using Plant;
using Zenject;

namespace DefaultNamespace.Systems.Plant
{
	public class CreatePlantSystem : IEcsInitSystem, IEcsRunSystem
	{
		private readonly List<PlantView.Pool> _palmPool;
		private readonly EcsPool<PoolIndexComponent> _poolPlantPoolIndexComponent;
		private readonly EcsPool<PlantPositionComponent> _poolPlantPosiiton;
		private readonly EcsPool<SpawnedComponent> _poolSpawnedComponent;
		private EcsFilter _filterNewPlant;

		public CreatePlantSystem(DiContainer container, EcsWorld world)
		{
			_palmPool = container.ResolveAll<PlantView.Pool>();
			_poolPlantPoolIndexComponent = world.GetPool<PoolIndexComponent>();
			_poolPlantPosiiton = world.GetPool<PlantPositionComponent>();
			_poolSpawnedComponent = world.GetPool<SpawnedComponent>();
		}
		
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			_filterNewPlant = world.Filter<PlantTypeComponent>()
				.Inc<PlantPositionComponent>()
				.Inc<PoolIndexComponent>()
				.Exc<SpawnedComponent>()
				.End();
		}

		public void Run(IEcsSystems systems)
		{
			if (_filterNewPlant.GetEntitiesCount() == 0)
				return;

			foreach (var entity in _filterNewPlant)
			{
				var poolIndex = _poolPlantPoolIndexComponent.Get(entity).Value;
				var posiiton = _poolPlantPosiiton.Get(entity).Value;
				var plantView = _palmPool[poolIndex].Spawn();
				plantView.transform.position = posiiton;
				_poolSpawnedComponent.Add(entity);
			}
		}
	}
}