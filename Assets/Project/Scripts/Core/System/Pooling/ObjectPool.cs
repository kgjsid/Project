using System.Collections.Generic;
using UnityEngine;

namespace Core.System.Pooling
{
    public abstract class ObjectPoolBase 
    {
        public abstract void ReturnObject(IPoolable obj);
    }

    public class ObjectPool<T> : ObjectPoolBase where T : Component, IPoolable
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Queue<T> pool = new Queue<T>();

        public ObjectPool(T prefab, Transform parent, int capacity = 4)
        {
            this.prefab = prefab;
            this.parent = parent;

            for(int i = 0; i < capacity; i++)
            {
                T obj = CreateNew();
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T GetPool()
        {
            T obj = pool.Count > 0 ? pool.Dequeue() : null;

            obj.gameObject.SetActive(true);
            obj.OnSpawn();
            return obj;
        }

        public void ReturnPool(T obj)
        {
            obj.OnDespawn();
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }

        public override void ReturnObject(IPoolable obj)
        {
            ReturnPool((T)obj);
        }

        private T CreateNew()
        {
            return Object.Instantiate(prefab, parent);
        }
    }
}