using System;
using CalongeCore.SoundManager;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsDictionary")]
public class SoundsDictionary : ScriptableObject
{
    public SoundTuple[] allSoundsTuples;
}

[Serializable]
public struct SoundTuple
{
    public SoundID id;
    public AudioClip audioClip;
}