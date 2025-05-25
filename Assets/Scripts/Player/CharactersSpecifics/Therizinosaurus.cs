using System.Collections;
using UnityEngine;

public class Therizinosaurus : CharacterBase
{
    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        for (int i = 0; i < attackCount; i++)
        {
            if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                activeCollider.SetActive(true);
                onAttack?.Invoke(damage);
                yield return new WaitForSeconds(enableDuration);
                activeCollider.SetActive(false);
                yield return new WaitForSeconds(disableDelay);
            }
            yield return new WaitForSeconds(1f / stats.attacksPerSecond - enableDuration - disableDelay);
        }
    }

    public override IEnumerator PerformSpecial(System.Action<float> onSpecial)
    {
        float clawDamage = 5f; // 4 * 5 = 20 as per GDD
        for (int i = 0; i < 4; i++)
        {
            if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                activeCollider.SetActive(true);
                onSpecial?.Invoke(clawDamage); // Apply 5 damage per slash with knockback
                yield return new WaitForSeconds(enableDuration / 2f); // Faster slashes
                activeCollider.SetActive(false);
                yield return new WaitForSeconds(disableDelay / 2f);
            }
        }
    }
}