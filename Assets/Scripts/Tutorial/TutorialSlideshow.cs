using System.Collections;
using UnityEngine;

public class TutorialSlideshow : MonoBehaviour
{
    [Tooltip("Slides in order (images, panels, etc.)")]
    [SerializeField] private GameObject[] slides;
    
    [Tooltip(" Objects to enable/disable when tutorial ends")]
    [SerializeField] private GameObject tutorialEnemy;
    [SerializeField] private GameObject goSign;
    [SerializeField] private GameObject startCollider;
    [SerializeField] private GameObject playersHUD;


    [Tooltip("Time each slide is visible (seconds)")]
    [SerializeField] private float slideDuration = 3f;

    [Tooltip("Start automatically on play")]
    [SerializeField] private bool autoStart = true;
    
    [SerializeField] public bool running { get; private set; } = true;
    void Start()
    {
        if (autoStart)
            StartCoroutine(RunSlideshow());
        else
            Destroy(tutorialEnemy);
    }

    public void Begin() => StartCoroutine(RunSlideshow());

    private IEnumerator RunSlideshow()
    {
        playersHUD?.SetActive(false);
        
        // Hide all at start
        foreach (var slide in slides) 
                slide?.SetActive(false);

        foreach (var slide in slides)
        {
            slide?.SetActive(true);
            
            yield return new WaitForSeconds(slideDuration);
            
            slide?.SetActive(false);
        }
        
        // Enable Game Start
        running = false;
        Destroy(tutorialEnemy);
        
        goSign?.SetActive(true);
        startCollider?.SetActive(true);
        playersHUD?.SetActive(true);
        
        Debug.Log("Tutorial finished");
    }
}