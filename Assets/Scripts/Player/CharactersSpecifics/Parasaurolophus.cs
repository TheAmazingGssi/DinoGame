using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Parasaurolophus : CharacterBase
{
    [SerializeField] private float specialAttackRange = 3f; // GDD: 3 units
    [SerializeField] private float specialActivationTime = 0.2f; // Brief activation

    public MeleeDamage SpecialMeleeDamage => specialMeleeDamage;

    public override IEnumerator PerformAttack(float damage, UnityAction<float> onAttack)
    {
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
            activeCollider.SetActive(true);
            onAttack?.Invoke(damage);
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
            yield return new WaitForSeconds(disableDelay);
        }
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (specialColliderGO == null) yield break;

        IsPerformingSpecialMovement = true; // Block movement
        specialColliderGO.SetActive(true);
        onSpecial?.Invoke(stats.specialAttackDamage);
        yield return new WaitForSeconds(specialActivationTime);
        specialColliderGO.SetActive(false);
        IsPerformingSpecialMovement = false; // Resume movement
    }
}