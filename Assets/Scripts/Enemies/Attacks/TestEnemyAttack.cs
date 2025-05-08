using UnityEngine;

public class TestEnemyAttack : EnemyAttack
{
    private const string Player = "Player";

    private bool isPlayerInRange = false;
    private PlayerCombatManager combatManager;
    protected override void ApplyDamage()
    {
        if(combatManager && isPlayerInRange)
        {
            combatManager.TakeDamage(new DamageArgs { Damage = manager.EnemyData.BaseDamage });
        }
    }

    protected override void HandleAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, manager.PlayerTransform.position);

        if (distanceToPlayer <= manager.EnemyData.AttackRange)
        {
            base.HandleAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.tag == "Player")
        {
            Debug.Log("Player Found");

            combatManager = collision.gameObject.GetComponent<PlayerCombatManager>();
            Debug.Log(combatManager);

            if (combatManager)
            {
                Debug.Log("combat manager found");
                isPlayerInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == CompareTag("Player"))
        {
            combatManager = null;
            isPlayerInRange = false;
        }
    }
}
