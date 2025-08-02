using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private int specialHitCount = 4; 
    [SerializeField] private float specialHitInterval = 0.15f; 

    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
    }
    
    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;
        
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeMeleeDamage?.PrepareDamage(stats.specialAttackDamage / specialHitCount, true, _mainPlayerController.transform, _mainPlayerController);
        activeCollider.SetActive(true);
        animController.SpecialVfxAnimator.gameObject.transform.position = activeCollider.transform.position;
        animController.specialVfxRenderer.flipX = !facingRight;
        animController.SpecialVfxAnimator.SetTrigger("Play");
        
        for (int i = 0; i < specialHitCount; i++)
        {
            activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage / specialHitCount, true, transform, _mainPlayerController);
            yield return new WaitForSeconds(specialHitInterval);
        }

        activeCollider.SetActive(false);
    }
}