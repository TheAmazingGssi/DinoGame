using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
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