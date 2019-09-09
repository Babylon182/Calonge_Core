using UnityEngine;
using System.Collections.Generic;
using System;
using CalongeCore.Events;
using CalongeCore.Managers;
using UnityEngine.Audio;

namespace CalongeCore.SoundManager
{
    public class SoundManager : SingletonWithDictionary<SoundManager, SoundID, AudioClip>
    {
        [SerializeField]
        private SoundsDictionary soundsDictionary;

        [SerializeField]
        private AudioMixerGroup defaultAudioMixer;

        private List<AudioSource> channels = new List<AudioSource>();
        private GameObject channelsGo;
        public int channelAmount;

        private const string AUDIO_CHANNELS = "AUDIO CHANNELS";
        private const string CHANNELS = "Channel";

        protected override void Awake()
        {
            base.Awake();
            EventsManager.SubscribeToEvent<SoundEvent>(OnSoundEvent);
            channelsGo = new GameObject(AUDIO_CHANNELS);
            channelsGo.transform.parent = transform;

            for (int i = 0; i < channelAmount; i++)
            {
                MakeChannel();
            }
        }

        private void OnDestroy()
        {
            EventsManager.UnsubscribeToEvent<SoundEvent>(OnSoundEvent);
        }

        public override void FillDictionary()
        {
            for (int index = soundsDictionary.allSoundsTuples.Length - 1; index >= 0; index--)
            {
                var currentC = soundsDictionary.allSoundsTuples[index];
                if (!dictionary.ContainsKey(currentC.id))
                {
                    dictionary.Add(currentC.id, currentC.audioClip);
                }
#if UNITY_EDITOR
                else
                {
                    LogWarningRepeatedElement(currentC.id.ToString(), index);
                }
#endif
            }
        }

        private void OnSoundEvent(SoundEvent gameEvent)
        {
            Play(gameEvent.id, gameEvent.position);
        }

        public bool Play(SoundID soundID, Vector3 position, float vol = 1, bool loop = false)
        {
            if (dictionary[soundID] == null)
                return false;

            AudioSource channel = GetFreeChannel();
            channel.transform.position = position;
            channel.clip = dictionary[soundID];
            channel.volume = vol;
            channel.loop = loop;
            channel.Play();

            return true;
        }

        public bool Stop(SoundID soundID)
        {
            if (dictionary[soundID] == null) return false;

            AudioSource channel = GetBusyChannel(dictionary[soundID]);

            if (channel == null) return false;

            channel.Stop();
            channel.clip = null;

            return true;
        }

        private AudioSource MakeChannel()
        {
            var newAudioSource = new GameObject(CHANNELS).AddComponent<AudioSource>();
            newAudioSource.transform.parent = channelsGo.transform;
            newAudioSource.outputAudioMixerGroup = defaultAudioMixer;
            channels.Add(newAudioSource);
            return newAudioSource;
        }

        private AudioSource GetFreeChannel()
        {
            for (int i = channels.Count - 1; i >= 0; i--)
            {
                if (!channels[i].isPlaying)
                    return channels[i];
            }

            return MakeChannel();
        }

        private AudioSource GetBusyChannel(AudioClip source)
        {
            for (int i = channels.Count - 1; i >= 0; i--)
            {
                if (channels[i].clip == source)
                    return channels[i];
            }

            return null;
        }
    }

    public class SoundEvent : IGameEvent
    {
        public SoundID id;
        public Vector3 position;
        public float vol;
        public bool loop;

        public SoundEvent()
        {
        }

        public SoundEvent(SoundID id, Vector3 position)
        {
            this.id = id;
            this.position = position;
        }

        public SoundEvent(SoundID id, Vector3 position, float vol, bool loop)
        {
            this.id = id;
            this.position = position;
            this.vol = vol;
            this.loop = loop;
        }

        public void Reset()
        {
            
        }
    }

    [Serializable]
    public struct Sounds
    {
        public SoundID id;
        public AudioClip audioClip;
    }

    //TODO OBVIAMENTE ESTO ESTA COMPLETAMENTE MAL, EL ENUM NO PUEDE ESTAR EN EL CALONGE CORE. ENCONTRAR UNA MANERA DE QUE
    //TODO QUE SEA LOCAL
    public enum SoundID
    {
        HeroDeath = 0,
        EnemyDeath = 1,
        
        HeroBasicWeapon = 10,
        
        None
    }
}