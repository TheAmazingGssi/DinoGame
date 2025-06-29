using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    private static readonly int IsDead = Animator.StringToHash("Dead");
    private const string Player = "Player";

    [Header("Components")]
    [SerializeField] private EnemyCombatManager combatManager;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private EnemyAttack[] attacks;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Drops")]
    [SerializeField] private GameObject healthItem;
    [SerializeField] private float healthItemDropChance = 0.25f;
    [Tooltip("Put -1 for number of players")]
    [SerializeField] private float healthDropAmount;

    private List<PlayerCombatManager> playersInRange = new List<PlayerCombatManager>();
    private Transform currentTarget;
    private PlayerCombatManager playerCombatManager;
    private PlayerCombatManager lastPlayerToDamage;
    private HashSet<PlayerCombatManager> playersWhoDealtDamage = new HashSet<PlayerCombatManager>();

    // Properties
    public PlayerCombatManager PlayerCombatManager => playerCombatManager;
    public Transform CurrentTarget => currentTarget;
    public Animator Animator => animator;
    public EnemyData EnemyData => enemyData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D RB => rb;
    public EnemyController EnemyController => enemyController;
    public EnemyCombatManager CombatManager => combatManager;
    public EnemyAttack[] Attacks => attacks;

    public event UnityAction<EnemyManager> OnDeath;

    private void Start()
    {
        if (healthDropAmount < 0)
        {
            healthDropAmount = PlayerEntity.PlayerList.Count;
        }
    }

    private void OnEnable()
    {
        combatManager.OnDeath += HandleDeath;
        combatManager.OnTakeDamage += HandleTakeDamage;
        combatManager.Initialize(enemyData.MaxHealth);
    }

    private void Update()
    {
        UpdatePlayerTracking();
        UpdateTargeting();
        HandleAttack();
    }

    private void UpdatePlayerTracking()
    {
        playersInRange.RemoveAll(player => player == null);
        playersWhoDealtDamage.RemoveWhere(player => player == null);
    }

    private void UpdateTargeting()
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

    private void HandleAttack()
    {
        if (attacks != null && attacks.Length > 0)
        {
            foreach (EnemyAttack attack in attacks)
            {
                if (attack != null)
                {
                    attack.TryAttack();
                }
            }
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

    #region Player Detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            PlayerCombatManager player = collision.GetComponent<PlayerCombatManager>();
            if (player != null && !playersInRange.Contains(player))
            {
                playersInRange.Add(player);
                Debug.Log($"Player {player.name} detected");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            PlayerCombatManager player = collision.GetComponent<PlayerCombatManager>();
            if (player != null)
            {
                playersInRange.Remove(player);
                playersWhoDealtDamage.Remove(player);
                Debug.Log($"Player {player.name} left detection");

                if (currentTarget == player.transform)
                {
                    ClearTarget();
                }
            }
        }
    }
    #endregion

    #region Targeting Logic
    public void OnPlayerDealtDamage(PlayerCombatManager player)
    {
        lastPlayerToDamage = player;
        playersWhoDealtDamage.Add(player);
        Debug.Log($"Player {player.name} dealt damage");

        if (playersInRange.Contains(player))
        {
            Debug.Log($"Player {player.name} that dealt damage is in range - switching target");
            SetTarget(player);
        }
    }

    private void SelectTarget()
    {
        if (playersInRange.Count == 0) return;

        PlayerCombatManager targetPlayer = null;

        if (lastPlayerToDamage != null && playersInRange.Contains(lastPlayerToDamage))
        {
            targetPlayer = lastPlayerToDamage;
            Debug.Log($"Targeting last player to deal damage: {targetPlayer.name}");
        }
        else
        {
            targetPlayer = playersInRange.OrderByDescending(p => p.CurrentHealth).First();
            Debug.Log($"Targeting player with highest health: {targetPlayer.name}");
        }

        if (targetPlayer != null)
        {
            SetTarget(targetPlayer);
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
    #endregion

    #region Death Handling
    private void HandleDeath(CombatManager combatManager)
    {
        OnDeath?.Invoke(this);
        animator.SetBool(IsDead, true);
        StartCoroutine(DeSpawn());

        if (Random.value < healthItemDropChance)
        {
            for (int i = 0; i < healthDropAmount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
                Vector3 spawnPosition = transform.position + (Vector3)randomOffset;
                Instantiate(healthItem, spawnPosition, Quaternion.identity);
            }
        }
    }

    private IEnumerator DeSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    #endregion

    private void OnValidate()
    {
        if (!combatManager)
        {
            combatManager = GetComponentInChildren<EnemyCombatManager>();
        }

        if (attacks == null || attacks.Length == 0)
        {
            attacks = GetComponentsInChildren<EnemyAttack>();
        }
    }
}