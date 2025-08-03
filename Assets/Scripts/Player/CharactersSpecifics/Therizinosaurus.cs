using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private int specialHitCount = 4; 
    [SerializeField] private float specialHitInterval = 0.15f; 
    [SerializeField] private float knockbackStrength = 40f; // adjustable in inspector

    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
    }
    
    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;
        
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;

        activeCollider.SetActive(true);

        // Play special VFX once at start
        animController.SpecialVfxAnimator.gameObject.transform.position = activeCollider.transform.position;
        animController.specialVfxRenderer.flipX = !facingRight;
        animController.SpecialVfxAnimator.SetTrigger("Play");
        
        for (int i = 0; i < specialHitCount; i++)
        {
            // Damage only — no knockback flag
            activeMeleeDamage?.PrepareDamage(
                stats.specialAttackDamage / specialHitCount,
                false, 
                _mainPlayerController.transform,
                _mainPlayerController
            );

            activeMeleeDamage?.ApplyDamage(
                stats.specialAttackDamage / specialHitCount,
                false, 
                transform,
                _mainPlayerController
            );

            yield return new WaitForSeconds(specialHitInterval);
        }

        // After all hits, apply one big knockback to all enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            GetComponentInChildren<CircleCollider2D>().radius,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                KnockbackHelper.ApplyKnockback(
                    hit.transform,
                    transform,
                    KnockbackHelper.GetKnockbackForceFromDamage(knockbackStrength, true),
                    KnockbackType.Normal
                );
            }
        }

        activeCollider.SetActive(false);
    }
}
