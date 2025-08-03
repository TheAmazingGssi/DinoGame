using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private int specialHitCount = 4; 
    [SerializeField] private float specialHitInterval = 0.15f; 
    [SerializeField] private float knockbackMultiplier = 0.3f; // small knockback strength multiplier

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

        for (int i = 0; i < specialHitCount; i++)
        {
            activeMeleeDamage.ClearHitList();
            
            // --- Play VFX for each hit ---
            animController.SpecialVfxAnimator.gameObject.transform.position = activeCollider.transform.position;
            animController.specialVfxRenderer.flipX = !facingRight;
            animController.SpecialVfxAnimator.SetTrigger("Play");

            // --- Damage ---
            float perHitDamage = stats.specialAttackDamage / specialHitCount;
            activeMeleeDamage?.PrepareDamage(
                perHitDamage,
                false, 
                _mainPlayerController.transform,
                _mainPlayerController
            );

            activeMeleeDamage?.ApplyDamage(
                perHitDamage,
                false, 
                transform,
                _mainPlayerController
            );

            // --- Small knockback on each hit ---
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
                        KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage * knockbackMultiplier, false)
                    );
                }
            }

            yield return new WaitForSeconds(specialHitInterval);
        }

        activeCollider.SetActive(false);
    }
}
