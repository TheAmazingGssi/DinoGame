using UnityEngine;
using System.Collections;

public class KnockbackManager : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackResistance = 1f;
    [SerializeField] private float maxKnockbackForce = 20f;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    private Rigidbody2D rb;
    private Vector2 originalVelocity;
    private bool isKnockedBack = false;
    private Coroutine knockbackCoroutine;
    
    // Events
    public System.Action<KnockbackData> OnKnockbackStart;
    public System.Action OnKnockbackEnd;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"KnockbackManager on {gameObject.name} requires a Rigidbody2D component!");
        }
    }
    
    /// <summary>
    /// Applies knockback to this object
    /// </summary>
    /// <param name="knockbackData">Data containing knockback parameters</param>
    public void ApplyKnockback(KnockbackData knockbackData)
    {
        if (rb == null || knockbackData.force <= 0) return;
        
        // Stop any existing knockback
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        
        // Calculate final knockback force with resistance
        float finalForce = Mathf.Min(knockbackData.force / knockbackResistance, maxKnockbackForce);
        
        // Store original velocity if not already in knockback
        if (!isKnockedBack)
        {
            originalVelocity = rb.linearVelocity;
        }
        
        // Create final knockback data
        KnockbackData finalData = new KnockbackData
        {
            force = finalForce,
            direction = knockbackData.direction.normalized,
            duration = knockbackData.duration > 0 ? knockbackData.duration : knockbackDuration,
            source = knockbackData.source,
            knockbackType = knockbackData.knockbackType
        };
        
        knockbackCoroutine = StartCoroutine(PerformKnockback(finalData));
    }
    
    /// <summary>
    /// Applies knockback with simple parameters
    /// </summary>
    public void ApplyKnockback(Vector2 direction, float force, float duration = 0f, Transform source = null)
    {
        KnockbackData data = new KnockbackData
        {
            direction = direction,
            force = force,
            duration = duration > 0 ? duration : knockbackDuration,
            source = source,
            knockbackType = KnockbackType.Normal
        };
        
        ApplyKnockback(data);
    }
    
    /// <summary>
    /// Applies radial knockback from a source position
    /// </summary>
    public void ApplyRadialKnockback(Vector2 sourcePosition, float force, float duration = 0f)
    {
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        ApplyKnockback(direction, force, duration);
    }
    
    private IEnumerator PerformKnockback(KnockbackData data)
    {
        isKnockedBack = true;
        OnKnockbackStart?.Invoke(data);
        
        float elapsedTime = 0f;
        Vector2 knockbackVelocity = data.direction * data.force;
        
        // Handle different knockback types
        switch (data.knockbackType)
        {
            case KnockbackType.Normal:
                yield return StartCoroutine(NormalKnockback(knockbackVelocity, data.duration));
                break;
            case KnockbackType.Grab:
                yield return StartCoroutine(GrabKnockback(data));
                break;
            case KnockbackType.Launch:
                yield return StartCoroutine(LaunchKnockback(knockbackVelocity, data.duration));
                break;
        }
        
        // Restore original velocity (modified for gameplay)
        rb.linearVelocity = originalVelocity * 0.5f; // Reduce original velocity after knockback
        
        isKnockedBack = false;
        OnKnockbackEnd?.Invoke();
        knockbackCoroutine = null;
    }
    
    private IEnumerator NormalKnockback(Vector2 knockbackVelocity, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = knockbackCurve.Evaluate(normalizedTime);
            
            rb.linearVelocity = Vector2.Lerp(knockbackVelocity, originalVelocity, 1f - curveValue);
            
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    
    private IEnumerator GrabKnockback(KnockbackData data)
    {
        if (data.source == null) yield break;
        
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;
        
        while (elapsedTime < data.duration)
        {
            if (data.source == null) break;
            
            Vector2 targetPosition = data.source.position;
            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / data.duration);
            
            rb.MovePosition(currentPosition);
            
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    
    private IEnumerator LaunchKnockback(Vector2 knockbackVelocity, float duration)
    {
        // Launch with upward component
        Vector2 launchVelocity = knockbackVelocity + Vector2.up * (knockbackVelocity.magnitude * 0.5f);
        rb.linearVelocity = launchVelocity;
        
        yield return new WaitForSeconds(duration);
    }
    
    /// <summary>
    /// Stops any current knockback effect
    /// </summary>
    public void StopKnockback()
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            isKnockedBack = false;
            OnKnockbackEnd?.Invoke();
            knockbackCoroutine = null;
        }
    }
    
    /// <summary>
    /// Checks if the object is currently being knocked back
    /// </summary>
    public bool IsKnockedBack => isKnockedBack;
    
    /// <summary>
    /// Sets the knockback resistance (higher = less knockback)
    /// </summary>
    public void SetKnockbackResistance(float resistance)
    {
        knockbackResistance = Mathf.Max(0.1f, resistance);
    }
}

[System.Serializable]
public struct KnockbackData
{
    public Vector2 direction;
    public float force;
    public float duration;
    public Transform source;
    public KnockbackType knockbackType;
    
    public KnockbackData(Vector2 dir, float f, float dur = 0.3f, Transform src = null, KnockbackType type = KnockbackType.Normal)
    {
        direction = dir;
        force = f;
        duration = dur;
        source = src;
        knockbackType = type;
    }
}

public enum KnockbackType
{
    Normal,     // Standard knockback with curve
    Grab,       // Pulls target towards source (like Spinosaurus chomp)
    Launch      // Launches target with upward component
}