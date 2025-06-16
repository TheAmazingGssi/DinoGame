using System.Collections;
using UnityEngine;

public class Parasaurolophus : CharacterBase
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
            // AOE: Activate both colliders
            rightMeleeColliderGO.SetActive(true);
            leftMeleeColliderGO.SetActive(true);
            onSpecial?.Invoke(stats.specialAttackDamage); // 20 damage
            yield return new WaitForSeconds(enableDuration);
            rightMeleeColliderGO.SetActive(false);
            leftMeleeColliderGO.SetActive(false);
        }
    }
}