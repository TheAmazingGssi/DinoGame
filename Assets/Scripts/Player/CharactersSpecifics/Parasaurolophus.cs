using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Parasaurolophus : CharacterBase
{
    [SerializeField] private float specialVfxActivationTime = 0.04f; 
    [SerializeField] private float restOfSpecialActivationTime = 0.46f; 
    
    public MeleeDamage SpecialMeleeDamage => specialMeleeDamage;
    
    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (specialColliderGO == null)
        {
            Debug.LogWarning("Special collider not found on Parasaurolophus!");
            yield break;
        }

        _mainPlayerController.ToggleIsAttacking();
        specialColliderGO.SetActive(true);

        // ✅ Pass correct controller here
        specialMeleeDamage?.PrepareDamage(stats.specialAttackDamage, true, _mainPlayerController.transform, _mainPlayerController);

        onSpecial?.Invoke(stats.specialAttackDamage);

        // Also apply immediate area damage if you want specials to hit instantly
        specialMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, _mainPlayerController.transform, _mainPlayerController);

        yield return new WaitForSeconds(specialVfxActivationTime);
        animController.SpecialVfxAnimator.SetTrigger("Play");
        yield return new WaitForSeconds(restOfSpecialActivationTime);

        specialColliderGO.SetActive(false);
        _mainPlayerController.ToggleIsAttacking();
    }

}