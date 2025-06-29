using System.Collections.Generic;
using UnityEngine;

public class EnemyAOEAttack : EnemyAttack
{
    [SerializeField] private float aoeRange = 5f;

    protected override float AttackRange => aoeRange;
    protected override bool IsPlayerInRange => GetPlayersInRange().Count > 0;

    protected override void ApplyDamage()
    {
        List<PlayerCombatManager> playersInRange = GetPlayersInRange();

        DamageArgs damageArgs = new DamageArgs{Damage = manager.EnemyData.BaseDamage};

        foreach (PlayerCombatManager player in playersInRange)
        {
            player.TakeDamage(damageArgs);
        }

        base.ApplyDamage();

        Debug.Log($"AOE attack dealt {manager.EnemyData.BaseDamage} damage to {playersInRange.Count} players");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRange);
    }
}