using System;
using Leopotam.EcsLite;
using Zenject;

namespace DefaultNamespace
{
	public class EcsBootstrap : IInitializable, ITickable, IDisposable
	{
		private readonly IEcsSystem[] systems;
		private EcsWorld _world;
		private EcsSystems _systems;

		public EcsBootstrap(IEcsSystem[] systems)
		{
			this.systems = systems;
		}

		public void Initialize()
		{
			_world = new EcsWorld();
			_systems = new EcsSystems(_world);

			foreach (var system in systems) 
				_systems.Add(system);

			_systems.Init();
		}

		public void Tick()
		{
			_systems.Run();
		}

		public void Dispose()
		{
			_systems.Destroy();
		}
	}
}