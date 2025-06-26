using System.Collections.Generic;
using UnityEngine;

public class EnemyAOEAttack : EnemyAttack
{
    [SerializeField] private float aoeRange = 5f;

    protected override float AttackRange => aoeRange;
    protected override bool IsPlayerInRange => GetPlayersInRange(AttackRange).Count > 0;

    protected override void ApplyDamage()
    {
        List<PlayerCombatManager> playersInRange = GetPlayersInRange(AttackRange);

        DamageArgs damageArgs = new DamageArgs{Damage = enemyData.BaseDamage, Source = gameObject};

        foreach (PlayerCombatManager player in playersInRange)
        {
            player.TakeDamage(damageArgs);
        }

        Debug.Log($"AOE attack dealt {enemyData.BaseDamage} damage to {playersInRange.Count} players");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRange);
    }
}