using UnityEngine;
using System.Collections.Generic;
using System;
using CalongeCore.Events;

namespace CalongeCore.SoundManager
{
    //TODO HACER MI PROPIO SOUND MANAGER O POR LO MENOS REFACTORIZAR ESTE
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;

        public static SoundManager Instance => instance;

        public Sounds[] sounds; //lista de sonidos
        public int channelAmount;
        private Dictionary<SoundID, AudioClip> dictionary = new Dictionary<SoundID, AudioClip>();
        private List<AudioSource> channels = new List<AudioSource>(); //canales de Audio
        private GameObject channelsGo;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            EventsManager.SubscribeToEvent<SoundEvent>(StartSound);
            channelsGo = new GameObject("AUDIO CHANNELS");
            channelsGo.transform.parent = transform;

            for (int i = 0; i < channelAmount; i++)
            {
                MakeChannel();
            }

            FillDictionary();
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

        private void StartSound(SoundEvent gameEvent)
        {
            Play(gameEvent.id, gameEvent.position);
        }

        void FillDictionary()
        {
            for (int i = sounds.Length - 1; i >= 0; i--)
            {
                var currentC = sounds[i];
                dictionary.Add(currentC.id, currentC.audioClip);
            }

            sounds = null;
        }

        private AudioSource MakeChannel()
        {
            var newAudioSource = new GameObject("Channel").AddComponent<AudioSource>();
            newAudioSource.transform.parent = channelsGo.transform;
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

        private AudioSource _GetBusyChannel(AudioClip source)
        {
            for (int i = channels.Count - 1; i >= 0; i--)
            {
                if (channels[i].clip == source)
                    return channels[i];
            }

            return null;
        }


        public bool Stop(SoundID soundID)
        {
            if (dictionary[soundID] == null) return false;

            AudioSource channel = _GetBusyChannel(dictionary[soundID]);

            if (channel == null) return false;

            channel.Stop();
            channel.clip = null;

            return true;
        }
    }

    public class SoundEvent : IGameEvent
    {
        public SoundID id;
        public Vector3 position;
    }

    [Serializable]
    public struct Sounds
    {
        public SoundID id;
        public AudioClip audioClip;
    }

    public enum SoundID
    {
        None
    }
}