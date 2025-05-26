using System.Collections;
using UnityEngine;

public class Spinosaurus : CharacterBase
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
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            // Increase collider size for long-range chomp
            BoxCollider2D collider = activeCollider.GetComponent<BoxCollider2D>();
            Vector2 originalSize = collider.size;
            collider.size *= 2f; // Double range for Chomp
            activeCollider.SetActive(true);
            onSpecial?.Invoke(stats.specialAttackDamage); // Apply damage with grab
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
            collider.size = originalSize; // Restore original size
        }
    }
}