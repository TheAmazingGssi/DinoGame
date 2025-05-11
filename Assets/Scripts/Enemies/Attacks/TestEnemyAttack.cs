using UnityEngine;

public class TestEnemyAttack : EnemyAttack
{
    private const string Player = "Player";
    private PlayerCombatManager combatManager;

    protected override bool IsPlayerInRange => combatManager != null;
    protected override void ApplyDamage()
    {
        if (combatManager)
        {
            combatManager.TakeDamage(new DamageArgs { Damage = manager.EnemyData.BaseDamage });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            Debug.Log("Player Found");
            combatManager = collision.gameObject.GetComponent<PlayerCombatManager>();
            if (combatManager)
            {
                Debug.Log("Combat manager found");
                TryAttack();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            Debug.Log("Player left attack range");
            combatManager = null;
        }
    }
}