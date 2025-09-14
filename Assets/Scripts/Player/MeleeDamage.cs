using UnityEngine;
using System.Collections.Generic;

public class MeleeDamage : MonoBehaviour
{
    [Header("Knockback Tuning")]
    [SerializeField] private float normalKnockbackMultiplier = 1f;   
    [SerializeField] private float specialKnockbackMultiplier = 1f;   
    
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
                Knockback = false
            });

            // knockback tuning - apply on all hits 
            float baseForce = KnockbackHelper.GetKnockbackForceFromDamage(currentDamage, isSpecialAttack);
            float tuned = baseForce * (isSpecialAttack ? specialKnockbackMultiplier : normalKnockbackMultiplier);
            KnockbackHelper.ApplyKnockback(
                other.transform,
                attackSource != null ? attackSource : transform,
                tuned
            );
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

        Collider2D[] hits = null;

        // Check if the object has a circle collider
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            hits = Physics2D.OverlapCircleAll(
                (Vector2)transform.position + circle.offset, 
                circle.radius,
                LayerMask.GetMask("Enemy")
            );
        }
        else
        {
            // Otherwise check if it has a box collider
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box != null)
            {
                hits = Physics2D.OverlapBoxAll(
                    (Vector2)transform.position + box.offset, 
                    box.size * 0.5f,   // half extents
                    transform.eulerAngles.z, 
                    LayerMask.GetMask("Enemy")
                );
            }
        }

        if (hits != null)
        {
            foreach (var hit in hits)
            {
                TryHitEnemy(hit);
            }
        }
    }

}
