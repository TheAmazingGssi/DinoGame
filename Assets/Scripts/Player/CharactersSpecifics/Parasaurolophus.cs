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

        GameObject specialColliderGO = facingRight ? specialColliderGORight : specialColliderGOLeft;
        MeleeDamage specialMeleeDamage = facingRight ? specialMeleeDamageRight : specialMeleeDamageLeft;
        if (!specialMeleeDamage)
        {
            Debug.LogWarning("MeleeDamage component missing on the active special collider!");
            yield break;
        }

        _mainPlayerController.ToggleIsAttacking();

        // Prepare BEFORE enabling so OnEnable/CheckExistingOverlaps uses correct damage
        specialMeleeDamage.PrepareDamage(
            stats.specialAttackDamage,
            true,
            specialColliderGO.transform,
            _mainPlayerController
        );

        specialColliderGO.SetActive(true);

        // Make physics aware of the new collider state, then wait a physics tick
        Physics2D.SyncTransforms();
        yield return new WaitForFixedUpdate();

        onSpecial?.Invoke(stats.specialAttackDamage);

        // Single burst hit now that overlaps are valid
        specialMeleeDamage.ApplyDamage(
            stats.specialAttackDamage,
            true,
            specialColliderGO.transform,
            _mainPlayerController
        );

        // VFX timing
        yield return new WaitForSeconds(specialVfxActivationTime);
        animController.TriggerSpecialVfx();
        yield return new WaitForSeconds(restOfSpecialActivationTime);

        specialColliderGO.SetActive(false);
        _mainPlayerController.ToggleIsAttacking();
    }
}