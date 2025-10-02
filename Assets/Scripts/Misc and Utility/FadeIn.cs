using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private Image blackScreen;   // drag the fullscreen Image here
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        if (blackScreen != null)
            StartCoroutine(FadeFromBlack());
        else
            Debug.LogWarning("FadeIn: No black screen Image assigned!");
    }

    private System.Collections.IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        Color c = blackScreen.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackScreen.color = c;
            yield return null;
        }

        // Ensure full transparency at the end
        c.a = 0f;
        blackScreen.color = c;
    }
}