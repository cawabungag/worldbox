using System;
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace XFlow.EcsLite {
    
    [Serializable]
    public struct EcsPackedEntity {
        //InternalId is public ONLY for Serialization purposes, You shouldn't use it. Use UNPACK
        public int InternalId;
        public int InternalGen;

        public override string ToString()
        {
            return $"EcsPackedEntity(Id={InternalId}, Gen={InternalGen})";
        }
    }

    public struct EcsPackedEntityWithWorld {
        internal int Id;
        internal int Gen;
        internal EcsWorld World;
        
#if DEBUG
        // For using in IDE debugger.
        internal object[] DebugComponentsView {
            get {
                object[] list = null;
                if (World != null && World.IsAlive () && World.IsEntityAliveInternal (Id) && World.GetEntityGen (Id) == Gen) {
                    World.GetComponents (Id, ref list);
                }
                return list;
            }
        }
        // For using in IDE debugger.
        internal int DebugComponentsCount {
            get {
                if (World != null && World.IsAlive () && World.IsEntityAliveInternal (Id) && World.GetEntityGen (Id) == Gen) {
                    return World.GetComponentsCount (Id);
                }
                return 0;
            }
        }

        // For using in IDE debugger.
        public override string ToString () {
            if (Id == 0 && Gen == 0) { return "Entity-Null"; }
            if (World == null || !World.IsAlive () || !World.IsEntityAliveInternal (Id) || World.GetEntityGen (Id) != Gen) { return "Entity-NonAlive"; }
            System.Type[] types = null;
            var count = World.GetComponentTypes (Id, ref types);
            System.Text.StringBuilder sb = null;
            if (count > 0) {
                sb = new System.Text.StringBuilder (512);
                for (var i = 0; i < count; i++) {
                    if (sb.Length > 0) { sb.Append (","); }
                    sb.Append (types[i].Name);
                }
            }
            return $"Entity-{Id}:{Gen} [{sb}]";
        }
#endif
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public static class EcsEntityExtensions {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntity PackEntity (this EcsWorld world, int entity) {
            EcsPackedEntity packed;
            packed.InternalId = entity;
            packed.InternalGen = world.GetEntityGen (entity);
            return packed;
        }

        public static ulong ToKey(int entity, int gen_)
        {
            var gen = (uint)gen_;
            ulong key = ((ulong)entity << 32) | gen;
            return key;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Unpack (this EcsPackedEntity packed, EcsWorld world, out int entity)
        {
            entity = -1;
            if (!world.IsAlive())
                return false;
            
            if (!world.IsEntityAliveInternal (packed.InternalId) || world.GetEntityGen (packed.InternalId) != packed.InternalGen)
            {
                var key = ToKey(packed.InternalId, packed.InternalGen);
                if (!world.RemappedEntities.TryGetValue(key, out EcsPackedEntity remappedEntity))
                    return false;

                packed.InternalId = remappedEntity.InternalId;
                packed.InternalGen = remappedEntity.InternalGen;

                return packed.Unpack(world, out entity);
            }
            
            entity = packed.InternalId;
            return true;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IsAlive (this in EcsPackedEntity packed, EcsWorld world) {
            if (!world.IsAlive () || !world.IsEntityAliveInternal (packed.InternalId) || world.GetEntityGen (packed.InternalId) != packed.InternalGen) 
                return false;
            return true;
        }
        
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static void InternalPackCustom (this ref EcsPackedEntity packed, int entity, int Gen)
        {
            packed.InternalId = entity;
            packed.InternalGen = Gen;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo (this in EcsPackedEntity a, in EcsPackedEntity b) {
            return a.InternalId == b.InternalId && a.InternalGen == b.InternalGen;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntityWithWorld PackEntityWithWorld (this EcsWorld world, int entity) {
            EcsPackedEntityWithWorld packedEntity;
            packedEntity.World = world;
            packedEntity.Id = entity;
            packedEntity.Gen = world.GetEntityGen (entity);
            return packedEntity;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool Unpack (this in EcsPackedEntityWithWorld packedEntity, out EcsWorld world, out int entity) {
            if (packedEntity.World == null || !packedEntity.World.IsAlive () || !packedEntity.World.IsEntityAliveInternal (packedEntity.Id) || packedEntity.World.GetEntityGen (packedEntity.Id) != packedEntity.Gen) {
                world = null;
                entity = -1;
                return false;
            }
            world = packedEntity.World;
            entity = packedEntity.Id;
            return true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo (this in EcsPackedEntityWithWorld a, in EcsPackedEntityWithWorld b) {
            return a.Id == b.Id && a.Gen == b.Gen && a.World == b.World;
        }
    }
}