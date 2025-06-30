using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private float specialAttackRange = 2f; // GDD: 2 units
    [SerializeField] private int specialHitCount = 4; // GDD: 4 hits
    [SerializeField] private float specialHitInterval = 0.15f; // Time between hits

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
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;

        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeCollider.SetActive(true);

        for (int i = 0; i < specialHitCount; i++)
        {
            onSpecial?.Invoke(stats.specialAttackDamage / specialHitCount);
            yield return new WaitForSeconds(specialHitInterval);
        }

        activeCollider.SetActive(false);
    }
}