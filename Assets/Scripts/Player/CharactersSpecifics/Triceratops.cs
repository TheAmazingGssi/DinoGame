using System.Collections;
using UnityEngine;

public class Triceratops : CharacterBase
{
    public override IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack)
    {
        float attackInterval = 1f / stats.attacksPerSecond; // 1.5 attacks/s = 0.6667s
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
            onSpecial?.Invoke(stats.specialAttackDamage); // 20 damage

            // Forward movement
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            rb.AddForce(direction * 200f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(enableDuration);
            rb.linearVelocity = Vector2.zero;

            activeCollider.SetActive(false);
        }
    }
}