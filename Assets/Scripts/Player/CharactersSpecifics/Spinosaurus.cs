using System.Collections;
using UnityEngine;

public class Spinosaurus : CharacterBase
{
    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        float attackInterval = 1f / stats.attacksPerSecond; // 1 attack/s = 1s
        for (int i = 0; i < attackCount; i++) // 3 attacks
        {
            if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                activeCollider.SetActive(true);
                onAttack?.Invoke(damage); // 10 damage
                yield return new WaitForSeconds(enableDuration);
                activeCollider.SetActive(false);
                yield return new WaitForSeconds(disableDelay);
                yield return new WaitForSeconds(attackInterval - enableDuration - disableDelay);
            }
        }
    }

    public override IEnumerator PerformSpecial(System.Action<float> onSpecial)
    {
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            activeCollider.SetActive(true);
            onSpecial?.Invoke(stats.specialAttackDamage); // 30 damage
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
        }
    }
}