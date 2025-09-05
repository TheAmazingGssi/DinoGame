using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Parasaurolophus : CharacterBase
{
    [SerializeField] private float specialVfxActivationTime = 0.04f; 
    [SerializeField] private float restOfSpecialActivationTime = 0.46f; 
    
    public MeleeDamage SpecialMeleeDamageRight => specialMeleeDamageRight;
    public MeleeDamage SpecialMeleeDamageLeft => specialMeleeDamageLeft;
    
    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (specialColliderGORight == null || specialColliderGOLeft == null)
        {
            Debug.LogWarning("Special colliders not assigned for Parasaurolophus!");
            yield break;
        }
        
        GameObject specialColliderGO = facingRight ? specialColliderGORight : specialColliderGOLeft;
        MeleeDamage specialMeleeDamage = facingRight ? specialMeleeDamageRight : specialMeleeDamageLeft;

        _mainPlayerController.ToggleIsAttacking();
        specialColliderGO.SetActive(true);

        specialMeleeDamage?.PrepareDamage(stats.specialAttackDamage, true, _mainPlayerController.transform, _mainPlayerController);

        onSpecial?.Invoke(stats.specialAttackDamage);

        // Damage
        specialMeleeDamage?.ApplyDamage(stats.specialAttackDamage, false, _mainPlayerController.transform, _mainPlayerController);

        // Knockback (once, strong)
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            GetComponentInChildren<CircleCollider2D>().radius,
            LayerMask.GetMask("Enemy")
        );
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                KnockbackHelper.ApplyKnockback
                (
                    hit.transform,
                    transform,
                    KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage * 1.5f, true) // strong knockback
                );
            }
        }
        yield return new WaitForSeconds(specialVfxActivationTime);
        animController.SpecialVfxAnimator.SetTrigger("Play");
        yield return new WaitForSeconds(restOfSpecialActivationTime);

        specialColliderGO.SetActive(false);
        _mainPlayerController.ToggleIsAttacking();
    }

}