using System;

namespace XFlow.EcsLite
{
    public struct CopyFromComponent
    {
        public struct pool
        {
            public IEcsPool Pool;
            public bool Checked;
        }
        public pool[] Pools;

        private Func<Type, bool> copyPoolPredicate;
        //public IEcsPool[] Pools;
        //public bool[] copy;
    }

    /*
    public static class CopyFromService
    {
        public static IEcsPool GetPool(this CopyFromComponent component)
        {
            var srcPoolId = srcWorld._entities[dataOffset + i];
            var srcPool = srcWorld._pools[srcPoolId];
            ref var poolData = ref buffer.Pools[srcPoolId];
            IEcsPool destPool = null;
            if (poolData.Checked)
            {
                destPool = poolData.Pool;
                if (destPool == null)
                    continue;
            }
            else
            {
                poolData.Checked = true;
                if (copyPoolPredicate != null && !copyPoolPredicate.Invoke(srcPool.GetComponentType()))
                {
                    continue;
                }
                destPool = GetOrCreatePoolByType(srcPool.GetComponentType());
                poolData.Pool = destPool;
            }
        }
    }*/
}