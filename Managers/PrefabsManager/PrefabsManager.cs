using CalongeCore.Events;
using CalongeCore.Managers;
using CalongeCore.ObjectsPool;
using UnityEngine;

namespace CalongeCore.ParticleManager
{
    public class PrefabsManager : SingletonWithDictionary<PrefabsManager, PrefabID, GameObject>
    {
        [SerializeField]
        private PrefabsDictionary prefabsDictionary;

        protected override void Awake()
        {
            base.Awake();
            EventsManager.SubscribeToEvent<ParticleEvent>(OnParticleEvent);
        }

        private void OnDestroy()
        {
            EventsManager.UnsubscribeToEvent<ParticleEvent>(OnParticleEvent);
        }

        public override void FillDictionary()
        {
            for (int index = prefabsDictionary.allPrefabsTuples.Length - 1; index >= 0; index--)
            {
                var currentC = prefabsDictionary.allPrefabsTuples[index];

                if (!dictionary.ContainsKey(currentC.id))
                {
                    dictionary.Add(currentC.id, currentC.prefab);
                }
#if UNITY_EDITOR
                else
                {
                    LogWarningRepeatedElement(currentC.id.ToString(), index);
                }
#endif
            }
        }

        private void OnParticleEvent(ParticleEvent gameEvent)
        {
            CreatePrefab(gameEvent.id, gameEvent.position, gameEvent.rotation);
        }

        private void CreatePrefab(PrefabID id, Vector3 position, Quaternion rotation)
        {
            GodPoolSingleton.Instance.Instantiate(dictionary[id], position, rotation);
        }
        
        private void CreatePrefab(PrefabID id)
        {
            GodPoolSingleton.Instance.Instantiate(dictionary[id], Vector3.zero, Quaternion.identity);
        }
    }

    public class ParticleEvent : IGameEvent
    {
        public PrefabID id;
        public Vector3 position;
        public Quaternion rotation;

        public ParticleEvent(PrefabID id, Vector3 position, Quaternion rotation)
        {
            this.id = id;
            this.position = position;
            this.rotation = rotation;
        }
    } 
    
    //TODO OBVIAMENTE ESTO ESTA COMPLETAMENTE MAL, EL ENUM NO PUEDE ESTAR EN EL CALONGE CORE. ENCONTRAR UNA MANERA DE QUE
    //TODO QUE SEA LOCAL
    public enum PrefabID
    {
        HeroDeath = 0,
        EnemyDeath = 1
    }
}