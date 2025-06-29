using System.Collections;
using UnityEngine;

public class Parasaurolophus : CharacterBase
{
    [SerializeField] private float specialAttackRange = 3f; // GDD: 3 units
    [SerializeField] private float specialActivationTime = 0.2f; // Brief activation

    public MeleeDamage SpecialMeleeDamage => specialMeleeDamage;

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
        if (specialColliderGO != null)
        {
            specialColliderGO.SetActive(true);
            onSpecial?.Invoke(stats.specialAttackDamage);
            yield return new WaitForSeconds(specialActivationTime);
            specialColliderGO.SetActive(false);
        }
    }
}