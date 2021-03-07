using System;
using System.Collections.Generic;

namespace XF.ChartLibrary.Utils
{
    public static class ObjectPool
    {
        public const int NoOwner = -1;
    }

    public class ObjectPool<T> where T : IPoolable
    {

        private static volatile int ids = 0;

        private int poolId;

        private int capacity;

        private object[] objects;

        private int objectsPointer;

        private readonly T modelObject;

        private float replenishPercentage;

        public int PoolId => poolId;

        private ObjectPool(int capacity, T obj)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Object Pool must be instantiated with a capacity greater than 0!", paramName: nameof(capacity));
            }
            this.capacity = capacity;
            objects = new object[this.capacity];
            objectsPointer = 0;
            modelObject = obj;
            replenishPercentage = 1.0f;
            RefillPool();
        }

        public float ReplenishPercentage
        {
            get => replenishPercentage;
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0f)
                {
                    value = 0f;
                }
                replenishPercentage = value;
            }
        }

        private void RefillPool(float value)
        {
            int portionOfCapacity = (int)(capacity * value);

            if (portionOfCapacity < 1)
            {
                portionOfCapacity = 1;
            }
            else if (portionOfCapacity > capacity)
            {
                portionOfCapacity = capacity;
            }

            for (int i = 0; i < portionOfCapacity; i++)
            {
                objects[i] = modelObject.Instantiate();
            }
            objectsPointer = portionOfCapacity - 1;
        }

        private void RefillPool() => RefillPool(replenishPercentage);

        public int Capacity => capacity;

        public int Count => objectsPointer + 1;

        private void ResizePool()
        {
            int oldCapacity = capacity;
            capacity *= 2;
            object[] temp = new object[capacity];
            for (int i = 0; i < oldCapacity; i++)
            {
                temp[i] = objects[i];
            }
            objects = temp;
        }

        public T Get()
        {
            if (objectsPointer == -1 && replenishPercentage > 0.0f)
            {
                RefillPool();
            }

            T result = (T)objects[objectsPointer];
            result.CurrentOwnerId = ObjectPool.NoOwner;
            objectsPointer--;

            return result;
        }

        /// <summary>
        ///   Recycle an instance of Poolable that this pool is capable of generating.
        /// The T instance passed must not already exist inside this or any other ObjectPool instance.
        /// </summary> 
        /// <param name="obj">An object of type T to recycle</param>
        public void Recycle(T obj)
        {
            if (obj.CurrentOwnerId != ObjectPool.NoOwner)
            {
                if (obj.CurrentOwnerId == poolId)
                {
                    throw new ArgumentException("The object passed is already stored in this pool!");
                }
                else
                {
                    throw new ArgumentException("The object to recycle already belongs to poolId " + obj.CurrentOwnerId + ".  Object cannot belong to two different pool instances simultaneously!");
                }
            }

            System.Threading.Interlocked.Increment(ref objectsPointer);
            if (objectsPointer >= objects.Length)
            {
                ResizePool();
            }

            obj.CurrentOwnerId = poolId;
            objects[objectsPointer] = obj;

        }

        /// <summary>
        ///  Recycle a List of Poolables that this pool is capable of generating.
        /// The T instances passed must not already exist inside this or any other ObjectPool instance.
        /// </summary>
        /// <param name="objects"> A list of objects of type T to recycle</param>
        public void Recycle(IList<T> objects)
        {
            int count = objects.Count + objectsPointer + 1;
            while (count > capacity)
            {
                ResizePool();
            }

            int objectsListSize = objects.Count;

            // Not relying on recycle(T object) because this is more performant.
            for (int i = 0; i < objectsListSize; i++)
            {
                T obj = objects[i];
                if (obj.CurrentOwnerId != ObjectPool.NoOwner)
                {
                    if (obj.CurrentOwnerId == poolId)
                    {
                        throw new ArgumentException("The object passed is already stored in this pool!");
                    }
                    else
                    {
                        throw new ArgumentException("The object to recycle already belongs to poolId " + obj.CurrentOwnerId + ".  Object cannot belong to two different pool instances simultaneously!");
                    }
                }
                obj.CurrentOwnerId = poolId;
                this.objects[objectsPointer + 1 + i] = obj;
            }
            objectsPointer += objectsListSize;
        }

        public static ObjectPool<T> Create(int withCapacity, T obj)
        {
            ObjectPool<T> result = new ObjectPool<T>(withCapacity, obj)
            {
                poolId = ids
            };
            System.Threading.Interlocked.Increment(ref ids);
            return result;
        }
    }

    public interface IPoolable
    {
        int CurrentOwnerId { get; set; }
        IPoolable Instantiate();
    }
}
