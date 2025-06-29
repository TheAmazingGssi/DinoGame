using UnityEngine;

[CreateAssetMenu(fileName = "VoiceClips", menuName = "Scriptable Objects/VoiceClips")]
public class VoiceClips : ScriptableObject
{
    public AudioClip[] Clips;

    public AudioClip PickRandom()
    {
        return Clips[Random.Range(0, Clips.Length)];
    }
}
