using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    private const string PLAYER = "Player";

    [SerializeField] private EnemyAttack[] attacks;
    [SerializeField] private EnemyManager manager;
    [SerializeField] private EnemyCombatManager combatManager;

    [SerializeField] private GameObject leftAttackCollider;
    [SerializeField] private GameObject rightAttackCollider;

    private List<PlayerCombatManager> playersInRange = new List<PlayerCombatManager>();
    private List<PlayerCombatManager> playersToRemove = new List<PlayerCombatManager>();

    private Transform currentTarget;
    private PlayerCombatManager playerCombatManager;
    private PlayerCombatManager lastPlayerToDamage;
    private bool isFacingRight = true;

    private bool isAttacking;
    public Transform CurrentTarget => currentTarget;
    public List<PlayerCombatManager> PlayersInRange => playersInRange;
    public PlayerCombatManager PlayerCombatManager => playerCombatManager;
    public bool IsFacingRight => isFacingRight;
    public GameObject LeftAttackCollider => leftAttackCollider;
    public GameObject RightAttackCollider => rightAttackCollider;
    public bool IsAttacking => isAttacking;

    private void Start()
    {
        combatManager.OnTakeDamage += HandleTakeDamage;

        if (leftAttackCollider != null)
            leftAttackCollider.SetActive(false);
        if (rightAttackCollider != null)
            rightAttackCollider.SetActive(false);
        isFacingRight = true;
    }

    private void Update()
    {
        UpdatePlayerTracking();
        UpdateFacing();
        HandleAttack();
    }

    private void LateUpdate()
    {
        UpdateTarget();
    }

    private void UpdateFacing()
    {
        if (currentTarget != null)
        {
            bool shouldFaceRight = currentTarget.position.x > transform.position.x;
            isFacingRight = shouldFaceRight;

        }
    }

    public void EnableAttackCollider()
    {
        if (isFacingRight && rightAttackCollider != null)
        {
            rightAttackCollider.SetActive(true);
            if (leftAttackCollider != null)
                leftAttackCollider.SetActive(false);
        }
        else if (!isFacingRight && leftAttackCollider != null)
        {
            leftAttackCollider.SetActive(true);
            if (rightAttackCollider != null)
                rightAttackCollider.SetActive(false);
        }
    }

    public void DisableAttackColliders()
    {
        if (leftAttackCollider != null)
            leftAttackCollider.SetActive(false);
        if (rightAttackCollider != null)
            rightAttackCollider.SetActive(false);
    }

    private void HandleAttack()
    {
        if (!isAttacking && currentTarget)
        {
            foreach (EnemyAttack attack in attacks)
            {
                attack.TryAttack();
            }
        }
    }

    public void ChangeAttackStatue(bool Attacking)
    {
        isAttacking = Attacking;

        if (Attacking)
        {
            EnableAttackCollider();
        }
        else
        {
            DisableAttackColliders();
        }
    }

    private void UpdatePlayerTracking()
    {
        playersInRange.RemoveAll(player => player == null);

        foreach (var player in playersToRemove)
        {
            if (playersInRange.Contains(player))
            {
                playersInRange.Remove(player);
                Debug.Log($"Player {player.name} removed from range");

                if (player == playerCombatManager)
                {
                    Debug.Log($"Current target {player.name} left range, clearing target");
                    ClearTarget();
                }
            }
        }
        playersToRemove.Clear();
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
        else if (playerCombatManager == null)
        {
            Debug.Log("Current target is null, selecting new target");
            ClearTarget();
            SelectTarget();
        }
    }

    private void SelectTarget()
    {
        if (playersInRange.Count == 0) return;

        PlayerCombatManager targetPlayer;

        if (lastPlayerToDamage != null && playersInRange.Contains(lastPlayerToDamage))
        {
            targetPlayer = lastPlayerToDamage;
            Debug.Log($"Targeting last player to deal damage: {targetPlayer.name}");
        }
        else
        {
            var validPlayers = playersInRange.Where(p => p != null).ToList();
            if (validPlayers.Count == 0)
            {
                Debug.LogWarning("No valid players found in range!");
                return;
            }

            targetPlayer = validPlayers.OrderByDescending(p => p.CurrentHealth).First();
            Debug.Log($"Targeting player with highest health: {targetPlayer.name}");
        }

        if (targetPlayer != null)
        {
            SetTarget(targetPlayer);
        }
    }

    public void OnPlayerDealtDamage(PlayerCombatManager player)
    {
        if (player == null) return;

        lastPlayerToDamage = player;

        if (playersInRange.Contains(player))
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
        DisableAttackColliders();
    }

    private void SetTarget(PlayerCombatManager player)
    {
        if (player == null) return;

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
        if (collision.transform.CompareTag(PLAYER))
        {
            PlayerCombatManager playerCombat = collision.GetComponent<PlayerCombatManager>();
            if (playerCombat != null && !playersInRange.Contains(playerCombat))
            {
                playersInRange.Add(playerCombat);
                Debug.Log($"Player {playerCombat.name} entered enemy range");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(PLAYER))
        {
            PlayerCombatManager playerCombat = collision.GetComponent<PlayerCombatManager>();
            if (playerCombat != null && playersInRange.Contains(playerCombat))
            {
                Collider2D detectionCollider = GetComponent<Collider2D>();
                if (detectionCollider != null)
                {
                    if (!detectionCollider.bounds.Intersects(collision.bounds))
                    {
                        playersToRemove.Add(playerCombat);
                        Debug.Log($"Player {playerCombat.name} marked to leave enemy range");
                    }
                }
                else
                {
                    playersToRemove.Add(playerCombat);
                    Debug.Log($"Player {playerCombat.name} marked to leave enemy range (fallback)");
                }
            }
        }
    }
}