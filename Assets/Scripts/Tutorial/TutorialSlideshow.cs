using System.Collections;
using UnityEngine;

public class TutorialSlideshow : MonoBehaviour
{
    [Tooltip("Slides in order (images, panels, etc.)")]
    [SerializeField] private GameObject[] slides;
    
    [Tooltip("Enemy used for tutorial purposes")]
    [SerializeField] private GameObject TutorialEnemy;
    
    [Tooltip("Go Sign to enable at the end of the tutorial")]
    [SerializeField] private GameObject GoSign;

    [Tooltip("Time each slide is visible (seconds)")]
    [SerializeField] private float slideDuration = 3f;

    [Tooltip("Start automatically on play")]
    [SerializeField] private bool autoStart = true;

    void Start()
    {
        if (autoStart)
            StartCoroutine(RunSlideshow());
    }

    public void Begin() => StartCoroutine(RunSlideshow());

    private IEnumerator RunSlideshow()
    {
        // Hide all at start
        foreach (var slide in slides) 
            if (slide) 
                slide.SetActive(false);

        foreach (var slide in slides)
        {
            if (slide)
                slide.SetActive(true);
            
            yield return new WaitForSeconds(slideDuration);
            
            if (slide) 
                slide.SetActive(false);
        }
        
        // Enable Game Start
        GameManager.Instance.InTutorial = false;
        Destroy(TutorialEnemy);
        
        if (GoSign) 
            GoSign.SetActive(true);
        
        Debug.Log("Tutorial finished");
    }
}