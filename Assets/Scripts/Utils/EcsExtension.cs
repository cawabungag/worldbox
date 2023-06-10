using Leopotam.EcsLite;

namespace DefaultNamespace.Utils
{
	public static class EcsExtension
	{
		public static T GetUnique<T>(this EcsWorld world) where T : struct
		{
			var filter = world.Filter<T>().End();
			var pool = world.GetPool<T>();
			var entity = filter.GetRawEntities()[0];
			return pool.Get(entity);
		}
		
		public static ref T GetUniqueRef<T>(this EcsWorld world) where T : struct
		{
			var filter = world.Filter<T>().End();
			var pool = world.GetPool<T>();
			var entity = filter.GetRawEntities()[0];
			return ref pool.Get(entity);
		}
	}
}