using System.Collections.Generic;

namespace XFlow.EcsLite
{
    public class AlwaysNull<T> {}

    public sealed class EventsSystem<T>:IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem where T: struct
    {
        private EcsWorld _world;
    
        public interface IComponentChangedListener
        {
            void OnComponentChanged(EcsWorld world, int entity, T data, bool newComponent);
        }
    
        public interface IComponentRemovedListener
        {
            void OnComponentRemoved(EcsWorld world, int entity, AlwaysNull<T> alwaysNull);
        }

        public interface IAnyComponentChangedListener
        {
            void OnAnyComponentChanged(EcsWorld world, int entity, T data, bool added);
        }
    
        public interface IAnyComponentRemovedListener
        {
            void OnAnyComponentRemoved(EcsWorld world, int entity, AlwaysNull<T> alwaysNull);
        }
    
        public struct ListenersComponent
        {
            public List<IComponentChangedListener> Changed;
            public List<IComponentRemovedListener> Removed;
        }
    
    
        private readonly List<IComponentChangedListener> _listenersBuffer = new List<IComponentChangedListener>(32);
        private readonly List<IComponentRemovedListener> _listenersRemovedBuffer = new List<IComponentRemovedListener>(32);
    
        private readonly List<IAnyComponentChangedListener> _anyListenersBuffer = new List<IAnyComponentChangedListener>(32);
        private readonly List<IAnyComponentRemovedListener> _anyListenersRemovedBuffer = new List<IAnyComponentRemovedListener>(32);
        
    
        private EcsFilter _filterChanges;
        private EcsFilter _filterRemoved;
    
        private EcsPool<T> _poolComponent;
        private EcsPool<EcsPool<T>.ChangedComponent> _poolChanged;
        private EcsPool<EcsPool<T>.AddedComponent> _poolAdded;
        private EcsPool<EcsPool<T>.RemovedComponent> _poolRemoved;
    
        public void Init(EcsSystems systems)
        {
            SetWorld(systems.GetWorld());
        }

        private void SetWorld(EcsWorld world)
        {
            _world = world;
            _filterChanges = _world.FilterBase().Inc<T>().IncChanges<T>().End();
            _filterRemoved = _world.FilterBase().IncRemoved<T>().End();
        
            _poolComponent = _world.GetPool<T>();
            _poolChanged = _world.GetPool<EcsPool<T>.ChangedComponent>();
            _poolAdded = _world.GetPool<EcsPool<T>.AddedComponent>();
        
            _poolRemoved = _world.GetPool<EcsPool<T>.RemovedComponent>();
        }
    
        public void Run(EcsSystems systems)
        {
            bool hasChangedComponents = _filterChanges.GetEntitiesCount() != 0;
            bool hasRemovedComponents = _filterRemoved.GetEntitiesCount() != 0;

            if (!hasChangedComponents && !hasRemovedComponents)
                return;//nothing changed


            //collect all AnyListeners
            _anyListenersRemovedBuffer.Clear();
            _anyListenersBuffer.Clear();
    
            var type = typeof(T);

            if (hasChangedComponents)
            {
                if (_world.anyListeners.TryGetValue(type, out IAnyComponentListeners ls))
                {
                    var listeners = ls as AnyComponentListeners<T>;
                    if (listeners.Changed != null)
                        _anyListenersBuffer.AddRange(listeners.Changed);
                }
            }
        
            if (hasRemovedComponents)
            {
                if (_world.anyListeners.TryGetValue(type, out IAnyComponentListeners ls))
                {
                    var listeners = ls as AnyComponentListeners<T>;
                    if (listeners.Removed != null)
                        _anyListenersRemovedBuffer.AddRange(listeners.Removed);
                }
            }


            EcsPool<ListenersComponent> poolListeners = _world.GetPool<ListenersComponent>();
        
            if (hasChangedComponents)
            {

                foreach (int entity in _filterChanges)
                {
                    _listenersBuffer.Clear();

                    if (poolListeners.Has(entity))
                    {
                        List<IComponentChangedListener> lst = poolListeners.InternalGetRef(entity).Changed;
                        if (lst != null)
                            _listenersBuffer.AddRange(lst);
                    }

                    bool added = _poolAdded.Has(entity);
                
                    T component = _poolComponent.InternalGetRef(entity);
                    foreach (IComponentChangedListener listener in _listenersBuffer)
                    {
                        listener.OnComponentChanged(_world, entity, component, added);
                    }

                    foreach (IAnyComponentChangedListener listener in _anyListenersBuffer)
                    {
                        listener.OnAnyComponentChanged(_world, entity, component, added);
                    }


                    _poolChanged.Del(entity);
                    _poolAdded.Del(entity);
                }
            
                _listenersBuffer.Clear();
            }


            if (hasRemovedComponents)
            {
                foreach (int entity in _filterRemoved)
                {
                    _listenersRemovedBuffer.Clear();

                    if (poolListeners.Has(entity))
                    {
                        List<IComponentRemovedListener> lst = poolListeners.InternalGetRef(entity).Removed;
                        if (lst != null)
                            _listenersRemovedBuffer.AddRange(lst);
                    }

                    foreach (IComponentRemovedListener listener in _listenersRemovedBuffer)
                    {
                        listener.OnComponentRemoved(_world, entity, null);
                    }

                    foreach (IAnyComponentRemovedListener listener in _anyListenersRemovedBuffer)
                    {
                        listener.OnAnyComponentRemoved(_world, entity, null);
                    }
                
                    _poolRemoved.Del(entity);
                }
            
                _listenersRemovedBuffer.Clear();
            }
        }

        public void PreInit(EcsSystems systems)
        {
            systems.GetWorld().SetEventsEnabled<T>();
        }
    }
}