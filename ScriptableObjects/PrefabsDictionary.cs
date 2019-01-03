using System;
using CalongeCore.ParticleManager;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsDictionary")]
public class PrefabsDictionary : ScriptableObject
{
    public PrefabTuple[] allPrefabsTuples;
}

[Serializable]
public struct PrefabTuple
{
    public PrefabID id;
    public GameObject prefab;
}
