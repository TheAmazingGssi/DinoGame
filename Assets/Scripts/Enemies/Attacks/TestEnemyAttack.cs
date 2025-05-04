using UnityEngine;

public class TestEnemyAttack : EnemyAttack
{
    private const string Player = "Player";

    [SerializeField] private EnemyManager manager;

    private bool isPlayerInRange = false;
    private CombatManager combatManager;
    protected override void ApplyDamage()
    {
        if(combatManager && isPlayerInRange)
        {
            combatManager.TakeDamage(new DamageArgs { Damage = manager.EnemyData.BaseDamage });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == CompareTag(Player))
        {
            combatManager = collision.gameObject.GetComponent<CombatManager>();
            if (combatManager)
            {
                isPlayerInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == CompareTag(Player))
        {
            combatManager = null;
            isPlayerInRange = false;
        }
    }
}
