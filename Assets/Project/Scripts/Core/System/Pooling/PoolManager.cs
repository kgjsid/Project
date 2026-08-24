using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.System.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        [Serializable]
        public class PoolConfig
        {
            public MonoBehaviour prefab;
            public int capacity;
        }

        private static PoolManager instance;
        public static PoolManager Instance { get { return instance; } }

        [SerializeField] private List<PoolConfig> configs = new List<PoolConfig>();

        private readonly Dictionary<Type, ObjectPoolBase> pools = new Dictionary<Type, ObjectPoolBase>();

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            CreatePools();
        }

        private void CreatePools()
        {
            foreach(var config in configs)
            {
                if (config.prefab == null) continue;

                if (config.prefab is not IPoolable) continue;

                Type type = config.prefab.GetType();
                if (pools.ContainsKey(type)) continue;

                ObjectPoolBase pool = CreatePool(config);
                pools[type] = pool;
            }
        }

        private ObjectPoolBase CreatePool(PoolConfig config)
        {
            Type type = config.prefab.GetType();

            Type poolType = typeof(ObjectPool<>).MakeGenericType(type);
            return (ObjectPoolBase)Activator.CreateInstance
                (poolType, config.prefab, transform, config.capacity);
        }

        public T Get<T>() where T : Component, IPoolable
        {
            if(!pools.TryGetValue(typeof(T), out var poolBase))
            {
                return null;
            }

            return ((ObjectPool<T>)poolBase).GetPool();
        }

        public void Return<T>(T obj) where T : Component, IPoolable
        {
            if (obj == null) return;

            if (!pools.TryGetValue(typeof(T), out var poolBase))
            {
                return;
            }

            ((ObjectPool<T>)poolBase).ReturnPool(obj);
        }

        public void Return(IPoolable obj)
        {
            if (obj == null) return;

            Type type = obj.GetType();
            if(!pools.TryGetValue(type, out var poolBase))
            {
                return;
            }

            poolBase.ReturnObject(obj);
        }
    }
}