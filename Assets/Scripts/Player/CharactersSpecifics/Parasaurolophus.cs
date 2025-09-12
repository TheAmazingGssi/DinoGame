using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Parasaurolophus : CharacterBase
{
    [SerializeField] private float specialVfxActivationTime = 0.04f; 
    [SerializeField] private float restOfSpecialActivationTime = 0.46f;
    //[SerializeField] private float absVfxStartPositionXValue = 0.15f;
    
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

        // Pick the correct side (uses the BoxCollider2D with MeleeDamage on that side)
        GameObject specialColliderGO = facingRight ? specialColliderGORight : specialColliderGOLeft;
        MeleeDamage specialMeleeDamage = facingRight ? specialMeleeDamageRight : specialMeleeDamageLeft;
        
        //float newVfxStartPos = facingRight ? absVfxStartPositionXValue : absVfxStartPositionXValue * -1;
        //Vector3 vfxstartPos = animController.SpecialVfxObject.gameObject.transform.position;
        //animController.SpecialVfxObject.transform.position = new Vector3(newVfxStartPos, vfxstartPos.y, vfxstartPos.z);
        
        _mainPlayerController.ToggleIsAttacking();
        specialColliderGO.SetActive(true);
        
        specialMeleeDamage?.PrepareDamage(
            stats.specialAttackDamage,
            true,                                   
            specialColliderGO.transform,             
            _mainPlayerController
        );

        onSpecial?.Invoke(stats.specialAttackDamage);

        // Apply damage using the active BoxCollider2D (MeleeDamage will handle per-target and knockback via attackSource)
        specialMeleeDamage?.ApplyDamage(
            stats.specialAttackDamage,
            false,                         // isSpecialAttack flag already set in PrepareDamage
            specialColliderGO.transform,             // keep source aligned with the active collider
            _mainPlayerController
        );
        

        yield return new WaitForSeconds(specialVfxActivationTime);
        animController.TriggerSpecialVfx();
        yield return new WaitForSeconds(restOfSpecialActivationTime);

        specialColliderGO.SetActive(false);
        _mainPlayerController.ToggleIsAttacking();
    }
    
}