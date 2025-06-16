
using System.Collections;
using UnityEngine;

public class Therizinosaurus : CharacterBase
{
    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        float attackInterval = 1f / stats.attacksPerSecond; // 3 attacks/s = 0.3333s
        for (int i = 0; i < attackCount; i++) // 2 attacks
        {
            if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
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
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            activeCollider.SetActive(true);
            // 4x5 damage
            for (int i = 0; i < 4; i++)
            {
                onSpecial?.Invoke(stats.specialAttackDamage / 4f); // 5 damage per hit
                yield return new WaitForSeconds(enableDuration / 4f);
            }
            activeCollider.SetActive(false);
        }
    }
}
