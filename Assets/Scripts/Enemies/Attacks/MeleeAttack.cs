using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        if (attackManager.PlayerCombatManager != null)
        {
            Debug.Log($"combat manager found");

            DamageArgs damageArgs = new DamageArgs{Damage = enemyData.BaseDamage,Source = gameObject};

            attackManager.PlayerCombatManager.TakeDamage(damageArgs);
        }

        base.ApplyDamage();
    }
}