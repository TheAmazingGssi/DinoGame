using System.Collections;
using UnityEngine;

public class Triceratops : CharacterBase
{
    [Header("Special Charge Settings")]
    [SerializeField] private float chargeDistance = 3f;
    [SerializeField] private float chargeSpeed = 7f;
    [SerializeField] private float chargeDamageDelay = 0.2f;
    [SerializeField] private float glideDistance = 0.5f;
    [SerializeField] private float glideDuration = 0.2f;

    [SerializeField] private SpriteRenderer spriteRenderer; // Assigned in Inspector

    private Coroutine specialRoutine;

    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        float attackInterval = 1f / stats.attacksPerSecond;

        for (int i = 0; i < attackCount; i++)
        {
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            activeCollider.SetActive(true);

            onAttack?.Invoke(damage);

            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
            yield return new WaitForSeconds(disableDelay);

            float remaining = attackInterval - enableDuration - disableDelay;
            if (remaining > 0) yield return new WaitForSeconds(remaining);
        }
    }

    public override IEnumerator PerformSpecial(System.Action<float> onSpecial)
    {
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;

        if (specialRoutine != null) StopCoroutine(specialRoutine);
        specialRoutine = StartCoroutine(ChargeForwardCoroutine(activeCollider, onSpecial));
        yield return specialRoutine;
    }

    private IEnumerator ChargeForwardCoroutine(GameObject activeCollider, System.Action<float> onSpecial)
    {
        // Determine direction based on visual facing
        Vector3 direction = transform.right.normalized;
        if (spriteRenderer != null && spriteRenderer.flipX)
            direction = -direction;

        float moved = 0f;

        activeCollider.SetActive(true);

        // Delay before damage hit
        yield return new WaitForSeconds(chargeDamageDelay);
        onSpecial?.Invoke(stats.specialAttackDamage);

        // Main charge forward
        while (moved < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime;
            transform.Translate(direction * step, Space.World);
            moved += step;
            yield return null;
        }

        activeCollider.SetActive(false);

        // Small glide after charge
        float glideMoved = 0f;
        float elapsed = 0f;

        while (elapsed < glideDuration && glideMoved < glideDistance)
        {
            float t = elapsed / glideDuration;
            float slideStep = Mathf.Lerp(chargeSpeed * 0.5f, 0f, t) * Time.deltaTime;
            transform.Translate(direction * slideStep, Space.World);
            glideMoved += slideStep;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
