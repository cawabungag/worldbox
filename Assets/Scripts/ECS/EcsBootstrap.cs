using System;
using XFlow.EcsLite;
using Zenject;

namespace DefaultNamespace
{
	public class EcsBootstrap : IInitializable, ITickable, IDisposable
	{
		private readonly IEcsSystem[] _ecsSystems;
		private EcsWorld _world;
		private EcsSystems _systems;

		public EcsBootstrap(IEcsSystem[] ecsSystems, EcsWorld ecsWorld)
		{
			_ecsSystems = ecsSystems;
			_world = ecsWorld;
		}

		public void Initialize()
		{
			_systems = new EcsSystems(_world);

			foreach (var system in _ecsSystems)
				_systems.Add(system);

			_systems.Init();
		}

		public void Dispose()
		{
			_systems.Destroy();
		}

		public void Tick()
		{
			_systems.Run();
		}
	}
}