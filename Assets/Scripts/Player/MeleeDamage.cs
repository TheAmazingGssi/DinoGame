using UnityEngine;
using System.Collections.Generic;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private MainPlayerController playerController;

    private HashSet<Collider2D> enemiesHit = new HashSet<Collider2D>();

    private float preparedDamage;
    private bool preparedSpecial;
    private Transform preparedSource;
    private MainPlayerController preparedController;
    private bool preparedGrab;

    private void OnEnable()
    {
        enemiesHit.Clear();
        CheckExistingOverlaps();
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHitEnemy(other);

    private void OnTriggerStay2D(Collider2D other) => TryHitEnemy(other);

    private void TryHitEnemy(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        bool alreadyHit = enemiesHit.Contains(other);

        if (!alreadyHit)
        {
            enemiesHit.Add(other);

            EnemyCombatManager enemyCombat = other.GetComponent<EnemyCombatManager>();
            if (enemyCombat != null)
            {
                enemyCombat.TakeDamage(new DamageArgs
                {
                    Damage = preparedDamage, 
                    SourceGO = playerController != null ? playerController.gameObject : null,
                    SourceMPC = playerController,
                    Knockback = preparedSpecial
                });
            }
        }

        // Only apply knockback once per activation per enemy
        if (preparedSpecial && !alreadyHit)
        {
            KnockbackHelper.ApplyKnockback(
                other.transform,
                preparedSource != null ? preparedSource : transform,
                KnockbackHelper.GetKnockbackForceFromDamage(preparedDamage * 3, true),
                preparedGrab ? KnockbackType.Grab : KnockbackType.Normal
            );
        }
    }

    private void CheckExistingOverlaps()
    {
        Collider2D[] hits = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        int count = GetComponent<Collider2D>().Overlap(filter, hits);

        for (int i = 0; i < count; i++)
            TryHitEnemy(hits[i]);
    }

    
    public void PrepareDamage(float damage, bool isSpecial, Transform source, MainPlayerController controller, bool isGrab = false)
    {
        preparedDamage = damage; 
        preparedSpecial = isSpecial;
        preparedSource = source;
        preparedGrab = isGrab;

        if (controller != null)
            playerController = controller;
        else if (playerController == null)
            playerController = GetComponentInParent<MainPlayerController>();

        enemiesHit.Clear();
    }
    
    public void ApplyDamage(float damage, bool isSpecial, Transform source, MainPlayerController controller, bool isGrab = false)
    {
        PrepareDamage(damage, isSpecial, source, controller, isGrab);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
            TryHitEnemy(hit);
    }
}
