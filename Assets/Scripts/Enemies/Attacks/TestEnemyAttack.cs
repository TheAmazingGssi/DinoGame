using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAttack : EnemyAttack
{
    private const string Player = "Player";
    private PlayerCombatManager playerToAttack;

    protected override bool IsPlayerInRange => playerToAttack;
    protected override void ApplyDamage()
    {
        playerToAttack.TakeDamage(new DamageArgs { Damage = manager.EnemyData.BaseDamage });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            if(manager.PlayerCombatManager)
            {
                TryAttack();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            Debug.Log("Player left attack range");
        }
    }
}