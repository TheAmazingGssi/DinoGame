using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    private const string PLAYER = "Player";


    [SerializeField] private EnemyAttack[] attacks;
    [SerializeField] private EnemyManager manager;
    [SerializeField] private EnemyCombatManager combatManager;


    private List<PlayerCombatManager> playersInRange = new List<PlayerCombatManager>();

    private Transform currentTarget;
    private PlayerCombatManager playerCombatManager;
    private PlayerCombatManager lastPlayerToDamage;

    private bool isAttacking;
    public Transform CurrentTarget => currentTarget;
    public List<PlayerCombatManager> PlayersInRange => playersInRange;

    public PlayerCombatManager PlayerCombatManager => playerCombatManager;



    private void Start()
    {
        combatManager.OnTakeDamage += HandleTakeDamage;
    }

    private void Update()
    {
        UpdateTarget();
        UpdatePlayerTracking();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if(!isAttacking && currentTarget)
        {
            foreach(EnemyAttack attack in attacks)
            {
                attack.TryAttack();
            }
        }
    }

    public void ChangeAttackStatue(bool Attacking)
    {
        isAttacking = Attacking;
    }

    private void UpdatePlayerTracking()
    {
        playersInRange.RemoveAll(player => player == null);
    }

    private void UpdateTarget()
    {
        if (playersInRange.Count == 0)
        {
            ClearTarget();
            return;
        }

        if (currentTarget == null)
        {
            SelectTarget();
        }
        else if (!playersInRange.Contains(playerCombatManager))
        {
            Debug.Log($"Current target {playerCombatManager.name} left range, selecting new target");
            ClearTarget();
            SelectTarget();
        }
    }
    private void SelectTarget()
    {
        if (playersInRange.Count == 0) return;

        //PlayerCombatManager targetPlayer = null;
        PlayerCombatManager targetPlayer;

        if (lastPlayerToDamage && playersInRange.Contains(lastPlayerToDamage))
        {
            targetPlayer = lastPlayerToDamage;
            Debug.Log($"Targeting last player to deal damage: {targetPlayer.name}");
        }
        else
        {
            targetPlayer = playersInRange.OrderByDescending(p => p.CurrentHealth).First();
            Debug.Log($"Targeting player with highest health: {targetPlayer.name}");
        }

        if (targetPlayer)
        {
            SetTarget(targetPlayer);
        }
    }
    public void OnPlayerDealtDamage(PlayerCombatManager player)
    {
        lastPlayerToDamage = player;
        Debug.Log($"Player {player.name} dealt damage");

        if (playersInRange.Contains(player)) //Should they have to be in range?
        {
            Debug.Log($"Player {player.name} that dealt damage is in range - switching target");
            SetTarget(player);
        }
    }

    private void HandleTakeDamage(DamageArgs damageArgs)
    {
        if (attacks != null)
        {
            foreach (EnemyAttack attack in attacks)
            {
                if (attack != null)
                {
                    attack.InterruptAttack();
                }
            }
        }
    }
    private void SetTarget(PlayerCombatManager player)
    {
        playerCombatManager = player;
        currentTarget = player.transform;
        Debug.Log($"Enemy targeting: {player.name}");
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            Debug.Log($"Clearing target: {currentTarget.name}");
        }
        currentTarget = null;
        playerCombatManager = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(PLAYER))
        {
            playersInRange.Add(collision.GetComponent<PlayerCombatManager>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER))
        {
            playersInRange.Remove(collision.GetComponent<PlayerCombatManager>());
        }
    }
}
