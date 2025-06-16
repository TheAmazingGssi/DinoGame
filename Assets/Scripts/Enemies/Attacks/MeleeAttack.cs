using UnityEngine;

public class EnemyMeleeAttack : EnemyAttack
{
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        Debug.Log($"tryint to damage player");

        if (manager.PlayerCombatManager != null)
        {
            Debug.Log($"combat manager found");

            DamageArgs damageArgs = new DamageArgs
            {
                Damage = enemyData.BaseDamage,
                Source = gameObject
            };

            manager.PlayerCombatManager.TakeDamage(damageArgs);
        }
    }
}