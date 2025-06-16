using UnityEngine;

/// Static helper class for applying knockback effects
public static class KnockbackHelper
{
    // Applies knockback to a target with automatic direction calculation
    public static void ApplyKnockback(Transform target, Transform source, float force, KnockbackType type = KnockbackType.Normal, float duration = 0.3f)
    {
        if (target == null || source == null) return;
        
        KnockbackManager knockbackManager = target.GetComponent<KnockbackManager>();
        if (knockbackManager == null) return;
        
        Vector2 direction = type == KnockbackType.Grab ? 
            (source.position - target.position).normalized : 
            (target.position - source.position).normalized;
        
        KnockbackData data = new KnockbackData(direction, force, duration, source, type);
        knockbackManager.ApplyKnockback(data);
    }

    
    // Applies radial knockback to all objects within a radius
    public static void ApplyRadialKnockback(Vector2 center, float radius, float force, LayerMask targetLayers, float duration = 0.3f)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center, radius, targetLayers);
        
        foreach (Collider2D target in targets)
        {
            KnockbackManager knockbackManager = target.GetComponent<KnockbackManager>();
            if (knockbackManager != null)
            {
                knockbackManager.ApplyRadialKnockback(center, force, duration);
            }
        }
    }
    
    /// <summary>
    /// Gets recommended knockback force based on damage dealt
    /// </summary>
    public static float GetKnockbackForceFromDamage(float damage, bool isSpecialAttack = false)
    {
        float baseForce = damage * 0.5f; // Base multiplier
        return isSpecialAttack ? baseForce * 2f : baseForce;
    }
}