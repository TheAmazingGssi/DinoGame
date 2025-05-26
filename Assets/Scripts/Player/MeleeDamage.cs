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
        CombatManager combatManager = other.GetComponent<CombatManager>();
        if (combatManager != null)
        {
            DamageArgs damageArgs = new DamageArgs { Damage = damage };
            combatManager.TakeDamage(damageArgs);
            Debug.Log($"Dealt {damage} damage to {other.name}{(isSpecial ? " (Special)" : "")}");

            // Subscribe to OnDeath for score if enemy
            if (other.CompareTag("Enemy") && playerController != null)
            {
                combatManager.OnDeath += (cm) => playerController.AddScore(10);
            }

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
            }
        }
    }
}