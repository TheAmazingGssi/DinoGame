using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    MainPlayerController playerController;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyCombatManager enemyCombat = other.GetComponent<EnemyCombatManager>();
            if (enemyCombat != null)
            {
                enemyCombat.TakeDamage(new DamageArgs { Damage = 0, Source = playerController});
            }
        }
    }

    public void ApplyDamage(float damage, bool isSpecial, Transform source, MainPlayerController controller = null, bool isGrab = false)
    {
        playerController = controller;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyCombatManager enemyCombat = hit.GetComponent<EnemyCombatManager>();
                if (enemyCombat != null)
                {
                    enemyCombat.TakeDamage(new DamageArgs { Damage = damage});
                    if (isSpecial && controller != null)
                    {
                        KnockbackHelper.ApplyKnockback(hit.transform, source, KnockbackHelper.GetKnockbackForceFromDamage(damage, true), isGrab ? KnockbackType.Grab : KnockbackType.Normal);
                    }
                    if (controller != null)
                    {
                        controller.AddScore((int)damage);
                    }
                }
            }
        }
    }
}