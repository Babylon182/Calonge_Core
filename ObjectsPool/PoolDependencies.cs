using System;
using UnityEngine;

namespace CalongeCore.ObjectsPool
{
    public class PoolObject
    {   
        private bool isActive;
        private Action initCallback;
        private Action disposeCallback;

        public PoolObject(GameObject poolGameObject, Action initCallback, Action disposeCallback)
        {
            this.PoolGameObject = poolGameObject;
            this.initCallback = initCallback;
            this.disposeCallback = disposeCallback;
            Id = poolGameObject.GetInstanceID();
        }

        public GameObject PoolGameObject { get;}
        public int Id { get;}

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
                    initCallback.Invoke();
                else 
                    disposeCallback.Invoke();
            }
        }
    }

    [Serializable]
    public class PoolAmount
    {
        public GameObject gameObject;
        public int ammount;
    }

    public interface IPoolable
    {
        void Init();
        void Dispose();
    }
}