using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackManager : MonoBehaviour
{
    [Header("How long a knockback lasts (sec)")]
    [SerializeField] private float defaultDuration = 0.3f;
    [Tooltip("Maximum time before we forcibly end knockback")]
    [SerializeField] private float maxDuration = 1f;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    public bool IsKnockedBack => isKnockedBack;

    private Coroutine kbRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Applies an impulse if not already knocked back.
    /// </summary>
    /// <param name="force">The impulse vector to apply.</param>
    /// <param name="duration">Optional duration; uses default if ≤ 0.</param>
    public void ApplyKnockback(Vector2 force, float duration = -1f)
    {
        if (isKnockedBack) return;

        if (duration <= 0f) duration = defaultDuration;
        duration = Mathf.Min(duration, maxDuration);

        isKnockedBack = true;
        // stop any existing motion
        rb.linearVelocity = Vector2.zero;
        // apply the impulse
        rb.AddForce(force, ForceMode2D.Impulse);

        if (kbRoutine != null)
            StopCoroutine(kbRoutine);
        kbRoutine = StartCoroutine(KnockbackRoutine(duration));
    }

    /// <summary>
    /// Ends the knockback early.
    /// </summary>
    public void EndKnockback()
    {
        if (!isKnockedBack) return;
        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
        if (kbRoutine != null)
        {
            StopCoroutine(kbRoutine);
            kbRoutine = null;
        }
    }

    private IEnumerator KnockbackRoutine(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        EndKnockback();
    }
}