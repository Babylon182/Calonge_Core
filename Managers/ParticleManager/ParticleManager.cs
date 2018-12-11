using System;
using System.Collections.Generic;
using CalongeCore.Events;
using UnityEngine;

namespace CalongeCore.ParticleManager
{
    //TODO HACER MI PROPIO PARTICLE MANAGER O POR LO MENOS REFACTORIZAR ESTE

    public class ParticleManager : MonoBehaviour
    {
        private static ParticleManager instance;
        public static ParticleManager Instance => instance;

        public ParticleAndGameobject[] makeDictionary;
        private Dictionary<ParticleID, GameObject> dictionary = new Dictionary<ParticleID, GameObject>();

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            FillDictionary();

            EventsManager.SubscribeToEvent<ParticleEvent>(PlaceParticle);
        }

        private void PlaceParticle(ParticleEvent gameEvent)
        {
            CreateParticle(gameEvent.id, gameEvent.position, gameEvent.rotation);
        }

        void FillDictionary()
        {
            for (int i = makeDictionary.Length - 1; i >= 0; i--)
            {
                var currentC = makeDictionary[i];
                dictionary.Add(currentC.particleID, currentC.prefab);
            }

            makeDictionary = null;
        }

        void CreateParticle(ParticleID id, Vector3 position, Quaternion rotation)
        {
            Instantiate(dictionary[id], position, rotation);
        }
    }

    public class ParticleEvent : IGameEvent
    {
        public ParticleID id;
        public Vector3 position;
        public Quaternion rotation;
    } 

    [Serializable]
    public struct ParticleAndGameobject
    {
        public ParticleID particleID;
        public GameObject prefab;
    }

    public enum ParticleID
    {
        None
    }
}