using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PulsingImage : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private float scaleSpeed = 2f;   
    [SerializeField] private float scaleAmount = 0.2f; 

    private Vector3 originalScale;

    private void OnEnable()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        originalScale = targetImage.rectTransform.localScale;
        StartCoroutine(Pulse());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (targetImage != null)
            targetImage.rectTransform.localScale = originalScale;
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            // Grow
            yield return ScaleTo(originalScale * (1 + scaleAmount));
            // Shrink
            yield return ScaleTo(originalScale);
        }
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        Vector3 start = targetImage.rectTransform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleSpeed;
            targetImage.rectTransform.localScale = Vector3.Lerp(start, targetScale, t);
            yield return null;
        }
    }
}