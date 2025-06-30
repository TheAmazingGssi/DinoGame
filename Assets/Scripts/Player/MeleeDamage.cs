using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private MainPlayerController playerController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyCombatManager enemyCombat = other.GetComponent<EnemyCombatManager>();
            if (enemyCombat != null)
            {
                enemyCombat.TakeDamage(new DamageArgs { Damage = 2, SourceGO = playerController.gameObject });
            }
        }
    }

    public void ApplyDamage(float damage, bool isSpecial, Transform source, MainPlayerController controller, bool isGrab = false)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyCombatManager enemyCombat = hit.GetComponent<EnemyCombatManager>();
                if (enemyCombat != null)
                {
                    enemyCombat.TakeDamage(new DamageArgs { Damage = damage, SourceGO = controller.gameObject });
                    if (isSpecial)
                    {
                        KnockbackHelper.ApplyKnockback(hit.transform, source, KnockbackHelper.GetKnockbackForceFromDamage(damage, true), isGrab ? KnockbackType.Grab : KnockbackType.Normal);
                    }
                }
            }
        }
    }
}