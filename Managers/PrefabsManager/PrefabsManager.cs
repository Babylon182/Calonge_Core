﻿using System;
using System.Collections.Generic;
using CalongeCore.Events;
using CalongeCore.ObjectsPool;
using UnityEngine;

namespace CalongeCore.ParticleManager
{
    public class PrefabsManager : Singleton<PrefabsManager>
    {
        [SerializeField]
        private PrefabsDictionary prefabsDictionary;
        
        private Dictionary<PrefabID, GameObject> allPrefabs = new Dictionary<PrefabID, GameObject>();

        protected override void Awake()
        {
            base.Awake();
            EventsManager.SubscribeToEvent<ParticleEvent>(PlaceParticle);
            FillDictionary();
        }

        private void PlaceParticle(ParticleEvent gameEvent)
        {
            CreatePrefab(gameEvent.id, gameEvent.position, gameEvent.rotation);
        }

        private void FillDictionary()
        {
            for (int i = prefabsDictionary.dictionary.Length - 1; i >= 0; i--)
            {
                var currentC = prefabsDictionary.dictionary[i];

                if (!allPrefabs.ContainsKey(currentC.id))
                {
                    allPrefabs.Add(currentC.id, currentC.prefab);
                }
                else
                {
                    Debug.Log($"<color=red>HEY FUCKER, You already have the key <color=blue>{currentC.id}</color> in the " +
                              $"Dictionary at index <color=blue>{i}</color>. PLEASE REMOVE IT.</color>");
                }
            }
        }

        private void CreatePrefab(PrefabID id, Vector3 position, Quaternion rotation)
        {
            GodPoolSingleton.Instance.Instantiate(allPrefabs[id], position, rotation);
        }
        
        private void CreatePrefab(PrefabID id)
        {
            GodPoolSingleton.Instance.Instantiate(allPrefabs[id], Vector3.zero, Quaternion.identity);
        }
    }

    public class ParticleEvent : IGameEvent
    {
        public PrefabID id;
        public Vector3 position;
        public Quaternion rotation;
    } 

    [Serializable]
    public struct PrefabTuple
    {
        public PrefabID id;
        public GameObject prefab;
    }

    public enum PrefabID
    {
        HeroDeath = 0,
        EnemyDeath = 1
    }
}