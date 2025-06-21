using UnityEngine;

public class EnemyMeleeDamage : MonoBehaviour
{
    [SerializeField] private EnemyData ed;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCombatManager playerCombat = other.GetComponent<PlayerCombatManager>();
            if (playerCombat != null)
            {
                // Deal damage
                DamageArgs damageArgs = new DamageArgs { Damage = ed.BaseDamage };
                playerCombat.TakeDamage(damageArgs);
            
                // Apply knockback
                float knockbackForce = KnockbackHelper.GetKnockbackForceFromDamage(ed.BaseDamage);
                KnockbackHelper.ApplyKnockback(other.transform, transform, knockbackForce);
            }
        }
    }
}
