using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private VoiceClips[] mySounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            Debug.LogError("GET COMPONENT GET COMPONENT GET COMPONENT -Eden xoxo"); //if you see this set the serialize field
        }
    }
    
    public void PlaySound(int index, float chance = 1)
    {
        PlaySound(mySounds[index].PickRandom(), chance);
    }
    
    public void PlaySound(VoiceClips data, float chance = 1)
    {
        PlaySound(data.PickRandom(), chance);
    }
    
    public void PlaySound(AudioClip clip, float chance = 1)
    {
        if (Random.Range(0f, 1f) > chance)
            return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
