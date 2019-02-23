using System.Collections.Generic;
using UnityEngine;

namespace CalongeCore.Managers
{
    public abstract class SingletonWithDictionary<TSingleton, TKey, TValue> : Singleton<TSingleton> where TSingleton : Singleton<TSingleton>
    {
        protected readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        public abstract void FillDictionary();

        protected override void Awake()
        {
            base.Awake();
            FillDictionary();
        }

#if UNITY_EDITOR
        protected void LogWarningRepeatedElement(string id, int index)
        {
            Debug.Log($"<color=red>HEY FUCKER, {typeof(TSingleton)} Here. You already have the key <color=blue>{id}</color> in the " +
                      $"Dictionary at index <color=blue>{index}</color>. PLEASE REMOVE IT.</color>", this.gameObject);
        }
#endif
    }
}