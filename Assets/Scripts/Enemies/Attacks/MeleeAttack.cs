using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        Debug.Log("Damage");
        PDamageArgs damageArgs = new PDamageArgs { Damage = manager.EnemyData.BaseDamage};

        manager.AttackManager.PlayerCombatManager.TakeDamage(damageArgs);


        base.ApplyDamage();
    }
}