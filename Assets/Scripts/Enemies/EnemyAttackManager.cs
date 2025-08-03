using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyAttackType
{
    Melee,
    Ranged,
    AOE
}

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
            EnemyAttack chosenAttack = null;
            EnemyAOEAttack aoeAttack = null;
            foreach (EnemyAttack attack in attacks)
            {
                if (attack is EnemyAOEAttack)
                {
                    aoeAttack = (EnemyAOEAttack)attack;
                    break;
                }
            }
            foreach (EnemyAttack attack in attacks)
            {
                if (attack is EnemyAOEAttack && attack.CanAttackNow())
                {
                    chosenAttack = attack;
                    break;
                }
            }
            if (chosenAttack == null)
            {
                foreach (EnemyAttack attack in attacks)
                {
                    if (!(attack is EnemyAOEAttack) && attack.CanAttackNow())
                    {
                        chosenAttack = attack;
                        break;
                    }
                }
            }

            chosenAttack?.TryAttack();
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

    public void OnAnimationComplete()
    {
        foreach (EnemyAttack attack in attacks)
        {
            if (attack.IsCurrentlyAttacking)
            {
                attack.OnAttackEnd();
                break;
            }
        }
    }

    private void UpdatePlayerTracking()
    {
        playersInRange.RemoveAll(player => player == null);

        foreach (var player in playersToRemove)
        {
            if (player != null && !IsPlayerStillInRange(player))
            {
                playersInRange.Remove(player);

                if (player == playerCombatManager)
                {
                    ClearTarget();
                }
            }
        }
        playersToRemove.Clear();
    }

    private bool IsPlayerStillInRange(PlayerCombatManager player)
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= manager.EnemyData.AttackRange;
    }

    private void UpdateTarget()
    {
        if (playersInRange.Count == 0)
        {
            ClearTarget();
            return;
        }
        if (currentTarget == null || playerCombatManager == null)
        {
            SelectTarget();
            return;
        }
        if (!playersInRange.Contains(playerCombatManager))
        {
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
            //Debug.Log($"Targeting last player to deal damage: {targetPlayer.name}");
        }
        else
        {
            var validPlayers = playersInRange.Where(p => p != null).ToList();
            if (validPlayers.Count == 0)
            {
                //Debug.LogWarning("No valid players found in range!");
                return;
            }

            targetPlayer = validPlayers.OrderByDescending(p => p.CurrentHealth).First();
            //Debug.Log($"Targeting player with highest health: {targetPlayer.name}");
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
            //Debug.Log($"Player {player.name} that dealt damage is in range - switching target");
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
        //Debug.Log($"Enemy targeting: {player.name}");
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            //Debug.Log($"Clearing target: {currentTarget.name}");
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
                //Debug.Log($"Player {playerCombat.name} entered enemy range");
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
                        //Debug.Log($"Player {playerCombat.name} marked to leave enemy range");
                    }
                }
                else
                {
                    playersToRemove.Add(playerCombat);
                    //Debug.Log($"Player {playerCombat.name} marked to leave enemy range (fallback)");
                }
            }
        }
    }

    // Add these methods to your EnemyAttackManager class:

    public bool IsCurrentlyAttacking
    {
        get
        {
            // Check if any of the attack components are currently attacking
            EnemyAttack[] attacks = GetComponents<EnemyAttack>();
            foreach (EnemyAttack attack in attacks)
            {
                if (attack.IsCurrentlyAttacking)
                    return true;
            }
            return false;
        }
    }

    public void InterruptAttack()
    {
        // Interrupt all active attacks
        EnemyAttack[] attacks = GetComponents<EnemyAttack>();
        foreach (EnemyAttack attack in attacks)
        {
            attack.InterruptAttack();
        }
    }
}