using UnityEngine;

public enum KnockbackType { Normal, Grab }

public static class KnockbackHelper
{
    public static void ApplyKnockback(Transform target, Transform source, float force, KnockbackType type)
    {
        Vector2 direction = (target.position - source.position).normalized;
        float duration = type == KnockbackType.Grab ? 0.5f : 0.3f;
        KnockbackManager knockback = target.GetComponent<KnockbackManager>();
        knockback?.ApplyKnockback(direction * force, duration);
    }

    public static float GetKnockbackForceFromDamage(float damage, bool isSpecial)
    {
        return isSpecial ? damage * 0.5f : damage * 0.3f;
    }
}