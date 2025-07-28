using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private int specialHitCount = 4; // GDD: 4 hits
    [SerializeField] private float specialHitInterval = 0.15f; // Time between hits

    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
    }
    
    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;

        //IsAttacking = true; 
        _mainPlayerController.ToggleIsAttacking();
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeCollider.SetActive(true);
        
        animController.specialVfx.SetTrigger("Play");
        
        for (int i = 0; i < specialHitCount; i++)
        {
            activeMeleeDamage?.ApplyDamage((stats.specialAttackDamage / specialHitCount), true, transform, null);
            //onSpecial?.Invoke(stats.specialAttackDamage / specialHitCount);
            yield return new WaitForSeconds(specialHitInterval);
        }

        activeCollider.SetActive(false);
        _mainPlayerController.ToggleIsAttacking();
        //IsAttacking = false; 
    }
}