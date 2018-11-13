using System.Collections.Generic;

namespace ObjectsPool
{
    public class GenericPool<T>
    {
        private Stack<PoolObject<T>> poolStack;
        private Stack<PoolObject<T>> activePoolStack;
        public delegate T CallbackFactory();

        private PoolObject<T>.PoolCallback init;
        private PoolObject<T>.PoolCallback dispose;
        private CallbackFactory factoryMethod;
        
        public GenericPool(int initialStock, CallbackFactory factoryMethod, PoolObject<T>.PoolCallback init, PoolObject<T>.PoolCallback dispose)
        {
            poolStack = new Stack<PoolObject<T>>();
            activePoolStack = new Stack<PoolObject<T>>();
            this.factoryMethod = factoryMethod;
            this.init = init;
            this.dispose = dispose;

            for (int i = 0; i < initialStock; i++)
            {
                poolStack.Push(new PoolObject<T>(this.factoryMethod(), this.init, this.dispose));
            }
        }

        public T GetObjectFromPool()
        {
            if (poolStack.Count > 0)
            {
                PoolObject<T> poolObject = poolStack.Pop();
                activePoolStack.Push(poolObject);
                return poolStack.Pop().GetObj;
            }

            PoolObject<T> newPoolObject = new PoolObject<T>(factoryMethod(), init, dispose);
            newPoolObject.IsActive = true;
            activePoolStack.Push(newPoolObject);
            return newPoolObject.GetObj;
        }

        public void ReturnObjectToPool(T obj)
        {
            if (activePoolStack.Count <= 0)
                return;

            PoolObject<T> poolObject = activePoolStack.Pop();
            poolObject.GetObj = obj;
            poolObject.IsActive = false;
            poolStack.Push(poolObject);
        }
    }
    
    public class PoolObject<T>
    {
        public delegate void PoolCallback(T obj);
        private PoolCallback init;
        private PoolCallback dispose;
        private bool isActive;

        public PoolObject(T obj, PoolCallback init, PoolCallback dispose)
        {        
            this.GetObj = obj;
            this.init = init;
            this.dispose = dispose;
        }

        public T GetObj { get; set; }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (isActive)
                    init(GetObj);
                else
                    dispose(GetObj);
            }
        }
    }
}