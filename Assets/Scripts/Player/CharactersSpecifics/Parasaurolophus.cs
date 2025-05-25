using System.Collections;
using UnityEngine;

public class Parasaurolophus : CharacterBase
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
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            // Use both colliders for area-of-effect roar
            rightMeleeColliderGO.SetActive(true);
            leftMeleeColliderGO.SetActive(true);
            onSpecial?.Invoke(stats.specialAttackDamage); // Apply damage with radial knockback
            yield return new WaitForSeconds(enableDuration);
            rightMeleeColliderGO.SetActive(false);
            leftMeleeColliderGO.SetActive(false);
        }
    }
}