using UnityEngine;
using System.Collections.Generic;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private MainPlayerController playerController;

    private HashSet<Collider2D> enemiesHit = new HashSet<Collider2D>();

    private float currentDamage;
    private bool isSpecialAttack;
    private Transform attackSource;
    private bool isGrabAttack;

    private void OnEnable()
    {
        enemiesHit.Clear();
        CheckExistingOverlaps();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitEnemy(other);
    }

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
                Damage = currentDamage,
                SourceGO = playerController != null ? playerController.gameObject : null,
                SourceMPC = playerController,
                Knockback = isSpecialAttack
            });

            // Apply knockback safely for specials
            if (isSpecialAttack)
            {
                KnockbackHelper.ApplyKnockback(
                    other.transform,
                    attackSource != null ? attackSource : transform,
                    KnockbackHelper.GetKnockbackForceFromDamage(currentDamage, true)
                );
            }
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
    
    public void ClearHitList()
    {
        enemiesHit.Clear();
    }

    public void PrepareDamage(float damage, bool special, Transform source, MainPlayerController controller, bool grab = false)
    {
        currentDamage = damage;
        isSpecialAttack = special;
        attackSource = source;
        isGrabAttack = grab;

        if (controller != null)
        {
            playerController = controller;
        }
        else if (playerController == null)
        {
            playerController = GetComponentInParent<MainPlayerController>();
        }
    }

    public void ApplyDamage(float damage, bool isSpecial, Transform source, MainPlayerController controller, bool isGrab = false)
    {
        PrepareDamage(damage, isSpecial, source, controller, isGrab);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            GetComponent<CircleCollider2D>().radius,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            TryHitEnemy(hit);
        }
    }
}
