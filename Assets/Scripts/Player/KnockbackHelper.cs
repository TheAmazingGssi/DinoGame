using UnityEngine;

public static class KnockbackHelper
{
    public static void ApplyKnockback(Transform target, Transform source, float force)
    {
        if (target == null || source == null) return;

        Vector2 dir = (target.position - source.position).normalized;
        float kbDuration = 0.3f;

        KnockbackManager kb = target.GetComponent<KnockbackManager>();
        if (kb != null)
        {
            kb.ApplyKnockback(dir * force, kbDuration);
        }
    }

    public static float GetKnockbackForceFromDamage(float damage, bool special)
    {
        // You can adjust this formula to taste
        return special ? damage * 0.7f : damage * 0.4f;
    }
}