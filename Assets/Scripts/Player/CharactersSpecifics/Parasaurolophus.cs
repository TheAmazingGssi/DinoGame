using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Parasaurolophus : CharacterBase
{
    [SerializeField] private float specialVfxActivationTime = 0.04f; 
    [SerializeField] private float restOfSpecialActivationTime = 0.46f; 
    
    public MeleeDamage SpecialMeleeDamage => specialMeleeDamage;

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (specialColliderGO == null)
        {
            Debug.LogWarning("Special collider not found on Parasaurolophus!");
            yield break;
        }

        IsAttacking = true; // Block movement
        specialColliderGO.SetActive(true);
        onSpecial?.Invoke(stats.specialAttackDamage);
        yield return new WaitForSeconds(specialVfxActivationTime);
        animController.specialVfx.SetTrigger("Play");
        yield return new WaitForSeconds(restOfSpecialActivationTime);
        specialColliderGO.SetActive(false);
        IsAttacking = false; // Resume movement
    }
}