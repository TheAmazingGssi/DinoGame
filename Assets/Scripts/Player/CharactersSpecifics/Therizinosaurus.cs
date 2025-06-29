using System.Collections;
using UnityEngine;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private float specialAttackRange = 2f; // GDD: 2 units
    [SerializeField] private int specialHitCount = 4; // GDD: 4 hits
    [SerializeField] private float specialHitInterval = 0.15f; // Time between hits

    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        float attackInterval = 1f / stats.attacksPerSecond;
        for (int i = 0; i < attackCount; i++)
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
                yield return new WaitForSeconds(attackInterval - enableDuration - disableDelay);
            }
        }
    }

    public override IEnumerator PerformSpecial(System.Action<float> onSpecial)
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