using UnityEngine;
using System.Collections.Generic;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private MainPlayerController playerController;

    //Track already-hit enemies
    private HashSet<Collider2D> enemiesHit = new HashSet<Collider2D>();

    
    private void OnEnable()
    {
        enemiesHit.Clear();
        CheckExistingOverlaps(); // catch stationary enemies right away
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitEnemy(other);
    }

    //detect if enemy is already inside and both are stationary
    private void OnTriggerStay2D(Collider2D other)
    {
        TryHitEnemy(other);
    }

    
    private void TryHitEnemy(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (enemiesHit.Contains(other)) return; 

        enemiesHit.Add(other);

        EnemyCombatManager enemyCombat = other.GetComponent<EnemyCombatManager>();
        if (enemyCombat != null)
        {
            enemyCombat.TakeDamage(new DamageArgs
            {
                Damage = 2,
                SourceGO = playerController.gameObject,
                SourceMPC = playerController
            });
        }
    }
    
    private void CheckExistingOverlaps()
    {
        Collider2D[] hits = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        int count = GetComponent<Collider2D>().Overlap(filter, hits);

        for (int i = 0; i < count; i++)
        {
            TryHitEnemy(hits[i]);
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
                    enemyCombat.TakeDamage(new DamageArgs { Damage = damage, SourceGO = controller.gameObject, SourceMPC = playerController });
                    if (isSpecial)
                    {
                        KnockbackHelper.ApplyKnockback(hit.transform, source, KnockbackHelper.GetKnockbackForceFromDamage(damage, true), isGrab ? KnockbackType.Grab : KnockbackType.Normal);
                    }
                }
            }
        }
    }
}
