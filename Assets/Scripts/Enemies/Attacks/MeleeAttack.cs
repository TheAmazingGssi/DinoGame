using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        if (manager.AttackManager.PlayerCombatManager != null)
        {
            Debug.Log($"combat manager found");

            DamageArgs damageArgs = new DamageArgs{Damage = manager.EnemyData.BaseDamage,Source = gameObject};

            manager.AttackManager.PlayerCombatManager.TakeDamage(damageArgs);
        }

        base.ApplyDamage();
    }
}