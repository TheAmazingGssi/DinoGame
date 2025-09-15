using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EndLevelSpotlight : MonoBehaviour
{
    [SerializeField] private Light2D[] globalLights; // assign your 2 global lights
    [SerializeField] private Light spotlight;       // optional: your local spotlight to keep on

    private float[] originalIntensity;
    private float spotlightVerticalOffset = 6.38f;
    private bool[] originalEnabled;
    private bool isDark;

    void Awake()
    {
        originalIntensity = new float[globalLights.Length];
        originalEnabled   = new bool[globalLights.Length];
        
        for (int i = 0; i < globalLights.Length; i++)
        {
            originalIntensity[i] = globalLights[i].intensity;
            originalEnabled[i]   = globalLights[i].enabled;
        }
    }

    public void EnableDark()
    {
        for (int i = 0; i < globalLights.Length; i++)
            globalLights[i].intensity = 0.05f;  // world goes dark

        if (spotlight) spotlight.enabled = true;  // only spotlight visible
        isDark = true;

        RelocateSpotlight();
    }

    private void RelocateSpotlight()
    {
        Vector3 playerPosition = GameManager.Instance.GetHighestScorePlayer().transform.position;
        transform.position = new Vector3(playerPosition.x, playerPosition.y + spotlightVerticalOffset, transform.position.z);
    }

    public void DisableDark()
    {
        for (int i = 0; i < globalLights.Length; i++)
        {
            globalLights[i].enabled  = originalEnabled[i];
            globalLights[i].intensity = originalIntensity[i];
        }
        isDark = false;
    }

    public void ToggleDark() => (isDark ? (System.Action)DisableDark : EnableDark)();
}