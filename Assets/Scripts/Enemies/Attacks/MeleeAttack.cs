using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        Debug.Log("Damage");
        DamageArgs damageArgs = new DamageArgs { Damage = manager.EnemyData.BaseDamage, Source = manager.CombatManager };

        manager.AttackManager.PlayerCombatManager.TakeDamage(damageArgs);


        base.ApplyDamage();
    }
}