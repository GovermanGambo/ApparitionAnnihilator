using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Audio Registry", fileName = "NewAudioRegistry", order = 0)]
public class AudioRegistry : ScriptableObject, IEnumerable<Sound>
{
    [SerializeField] private Sound[] sounds;
    
    public IEnumerator<Sound> GetEnumerator()
    {
        return sounds.ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Serializable]
public struct Sound
{
    [SerializeField] private string name;
    [SerializeField] private AudioClip clip;

    public string Name => name;
    public AudioClip Clip => clip;
}
