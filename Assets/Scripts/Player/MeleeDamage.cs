using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    private float damage;
    private bool isSpecial;
    private Transform playerTransform;
    private bool isSpinosaurusChomp;
    private MainPlayerController playerController;

    public void ApplyDamage(float damageAmount, bool isSpecialAttack, Transform player, MainPlayerController controller, bool isChomp = false)
    {
        damage = damageAmount;
        isSpecial = isSpecialAttack;
        playerTransform = player;
        playerController = controller;
        isSpinosaurusChomp = isChomp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyCombatManager combatManager = other.GetComponent<EnemyCombatManager>();
        if (combatManager != null)
        {
            DamageArgs damageArgs = new DamageArgs { Damage = damage };
            combatManager.TakeDamage(damageArgs);
            Debug.Log($"Player dealt {damage} damage to {other.name}{(isSpecial ? " (Special)" : "")}");

            // Subscribe to OnDeath for score if enemy
            if (other.CompareTag("Enemy") && playerController != null)
            {
                combatManager.OnDeath += (cm) => playerController.AddScore(10);
            }

            //old Knockback
            /*
            // Apply knockback or grab
            Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                if (isSpinosaurusChomp)
                {
                    Vector2 grabPosition = playerTransform.position;
                    enemyRb.position = Vector2.MoveTowards(enemyRb.position, grabPosition, 2f);
                    Debug.Log($"Grabbed {other.name} towards player");
                }
                else
                {
                    Vector2 knockbackDirection = (other.transform.position - playerTransform.position).normalized;
                    float knockbackForce = isSpecial ? 10f : 5f;
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                    Debug.Log($"Applied knockback to {other.name}");
                }
            }*/
            
            //New Knockback
            // Replace the knockback section in OnTriggerEnter2D with this:
            KnockbackManager knockbackManager = other.GetComponent<KnockbackManager>();
            if (knockbackManager != null)
            {
                if (isSpinosaurusChomp)
                {
                    // Use grab knockback for Spinosaurus chomp
                    KnockbackData grabData = new KnockbackData(
                        (playerTransform.position - other.transform.position).normalized,
                        5f, 0.5f, playerTransform, KnockbackType.Grab
                    );
                    knockbackManager.ApplyKnockback(grabData);
                }
                else
                {
                    // Normal knockback
                    float knockbackForce = KnockbackHelper.GetKnockbackForceFromDamage(damage, isSpecial);
                    KnockbackHelper.ApplyKnockback(other.transform, playerTransform, knockbackForce);
                }
            }
        }
    }
}