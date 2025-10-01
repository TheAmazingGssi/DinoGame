using System;
using System.Collections;
using UnityEngine;

public class CoopAttackSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float delay = 0.2f;
    
    public static CoopAttackSoundPlayer instance;

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
    
    public void PlaySound()
    {
        StartCoroutine(MultiPlaySound());
    }
    
    private IEnumerator MultiPlaySound()
    {
        audioSource.Play();
        yield return new WaitForSeconds(delay);
        audioSource.Play();
        yield return new WaitForSeconds(delay);
        audioSource.Play();
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}
