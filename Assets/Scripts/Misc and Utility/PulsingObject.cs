using UnityEngine;
using System.Collections;

public class TransformPulser : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 2f;      // How fast the pulse happens
    [SerializeField] private float pulseScaleAmount = 0.2f; // How much bigger than original size

    private Vector3 originalScale;

    private void OnEnable()
    {
        originalScale = transform.localScale;
        StartCoroutine(PulseRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.localScale = originalScale;
    }

    private IEnumerator PulseRoutine()
    {
        while (true)
        {
            // Grow
            yield return ScaleTo(originalScale * (1f + pulseScaleAmount));
            // Shrink
            yield return ScaleTo(originalScale);
        }
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * pulseSpeed;
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
    }
}