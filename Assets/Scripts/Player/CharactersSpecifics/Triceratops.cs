using System.Collections;
using UnityEngine;

public class Triceratops : CharacterBase
{
    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        for (int i = 0; i < attackCount; i++)
        {
            if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                activeCollider.SetActive(true);
                onAttack?.Invoke(damage); // Notify damage (with knockback in MeleeDamage)
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
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            activeCollider.SetActive(true);
            onSpecial?.Invoke(stats.specialAttackDamage); // Apply special damage with knockback
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
        }
    }
}