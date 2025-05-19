using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAttack : EnemyAttack
{
    private const string Player = "Player";
    private PlayerCombatManager playerToAttack;

    private List<PlayerCombatManager> players;

    protected override bool IsPlayerInRange => playerToAttack;
    protected override void ApplyDamage()
    {
        playerToAttack.TakeDamage(new DamageArgs { Damage = manager.EnemyData.BaseDamage });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            Debug.Log("Player Found");
            players.Add(collision.gameObject.GetComponent<PlayerCombatManager>());
            if (players.Count > 1)
            {
                Debug.Log("Combat manager found");
                for(int i = 0; i <= players.Count; i++)
                {
                    for (int j = i + 1; j <= players.Count; j++)
                    {
                        if (players[i].CurrentHealth > players[j].CurrentHealth)
                        {
                            players[i] = playerToAttack;
                        }
                        else
                        {
                            players[j] = playerToAttack;
                        }
                    }
                }
            }
            else if (players.Count == 1)
            {
                players[0] = playerToAttack;
            }
            if(playerToAttack)
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
            players.Remove(collision.gameObject.GetComponent<PlayerCombatManager>());
            if(players.Count <= 0)
            {
                playerToAttack = null;
            }
        }
    }
}