using System;
using UnityEngine;

public class SelectionSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public static SelectionSoundPlayer instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (!audioSource)
        {
            Debug.LogError("GET COMPONENT GET COMPONENT GET COMPONENT -Eden xoxo"); //if you see this set the serialize field
        }
    }
    
    public void PlaySelectionSound()
    {
        audioSource.Play();
    }
}
