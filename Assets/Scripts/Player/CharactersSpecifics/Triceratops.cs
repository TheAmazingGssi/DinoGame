using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Triceratops : CharacterBase
{
    [SerializeField] private float chargeDistance = 3f;
    [SerializeField] private float chargeSpeed = 13f;
    [SerializeField] private float glideDistance = 2.5f;
    [SerializeField] private float glideSpeed = 8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null || rb == null) yield break;

        IsAttacking = true;
        animController.terryParticleSystem.Play();

        // Select correct collider & damage component
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;

        Vector3 startPos = transform.position;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Vector3 targetPos = startPos + direction * chargeDistance;

        // Track all enemies hit during charge
        HashSet<Collider2D> enemiesHitDuringCharge = new HashSet<Collider2D>();

        // Enable collider for detection
        activeCollider.SetActive(true);

        // --- Charge phase ---
        float elapsed = 0f;
        float chargeDuration = chargeDistance / chargeSpeed;
        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, elapsed / chargeDuration);
            rb.MovePosition(newPos);

            // Check hits during charge
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                GetComponentInChildren<CircleCollider2D>().radius,
                LayerMask.GetMask("Enemy")
            );

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy") && !enemiesHitDuringCharge.Contains(hit))
                {
                    // Damage once
                    activeMeleeDamage?.PrepareDamage(stats.specialAttackDamage, false, _mainPlayerController.transform, _mainPlayerController);
                    activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, false, transform, _mainPlayerController);

                    // Large knockback immediately
                    KnockbackHelper.ApplyKnockback(
                        hit.transform,
                        transform,
                        KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage * 1.5f, true), // large knockback
                        KnockbackType.Normal
                    );

                    enemiesHitDuringCharge.Add(hit);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        // --- Glide phase ---
        startPos = transform.position;
        targetPos = startPos + direction * glideDistance;
        float glideDuration = glideDistance / glideSpeed;
        elapsed = 0f;

        while (elapsed < glideDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, elapsed / glideDuration);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);

        // Cleanup
        activeCollider.SetActive(false);
        animController.terryParticleSystem.Stop();
        IsAttacking = false;
    }
}
