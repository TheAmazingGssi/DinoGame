using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EndLevelSpotlight : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D spotlight;

    [Header("Behavior")]
    [SerializeField] private float spotlightVerticalOffset = 10.25f;

    [Header("Global Light Intensities (set in Inspector)")]
    [SerializeField] private float globalLightFullIntensity = 1.5f;   // value when not dark
    [SerializeField] private float globalLightDarkenedIntensity = 0.15f; // value when dark

    private bool isDark;

    public void EnableDark()
    {
        RelocateSpotlight();
       
        if (globalLight)
            globalLight.intensity = globalLightDarkenedIntensity;

        if (spotlight != null)
            spotlight.enabled = true;

        isDark = true;
    }

    public void DisableDark()
    {
        if (globalLight)
            globalLight.intensity = globalLightFullIntensity;  

        if (spotlight != null)
            spotlight.enabled = false;

        isDark = false;
    }

    public void ToggleDark() => (isDark ? (System.Action)DisableDark : EnableDark)();

    private void RelocateSpotlight()
    {
        Vector3 playerPosition = GameManager.Instance.GetHighestScorePlayer().transform.position;
        spotlight.transform.position = new Vector3(playerPosition.x, playerPosition.y + spotlightVerticalOffset, transform.position.z);
    }
}
