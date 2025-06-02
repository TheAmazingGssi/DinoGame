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
    [SerializeField] private EnemyAttack attack;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private PlayerTransformData playerTransform;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;

    private List<PlayerCombatManager> playersInRange = new List<PlayerCombatManager>();
    private Transform currentTarget;
    private PlayerCombatManager playerCombatManager;
    private PlayerCombatManager lastPlayerToDamage;

    public PlayerCombatManager PlayerCombatManager => playerCombatManager;
    public Transform CurrentTarget => currentTarget;
    public Animator Animator => animator;
    public EnemyData EnemyData => enemyData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D RB => rb;
    public EnemyController EnemyController => enemyController;
    public EnemyCombatManager CombatManager => combatManager;
    public PlayerTransformData PlayerTransform => playerTransform;
    public EnemyAttack Attack => attack;

    public event UnityAction<EnemyManager> OnDeath;

    private void OnEnable()
    {
        combatManager.OnDeath += HandleDeath;
        combatManager.Initialize(enemyData.MaxHealth);
    }

    private void Update()
    {
        UpdatePlayerTracking();
        UpdateTargeting();
        HandleAttackLogic();
    }

    private void UpdatePlayerTracking()
    {
        playersInRange.RemoveAll(player => player == null);
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
    }

    private void HandleAttackLogic()
    {

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

        if (playersInRange.Contains(player))
        {
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
        }
        else
        {
            targetPlayer = playersInRange.OrderBy(p => p.CurrentHealth).First();
        }

        SetTarget(targetPlayer);
    }

    private void SetTarget(PlayerCombatManager player)
    {
        playerCombatManager = player;
        currentTarget = player.transform;
        Debug.Log($"Enemy targeting: {player.name}");
    }

    private void ClearTarget()
    {
        currentTarget = null;
        playerCombatManager = null;
        lastPlayerToDamage = null;
    }
    #endregion

    #region Death Handling
    private void HandleDeath(CombatManager combatManager)
    {
        OnDeath?.Invoke(this);
        animator.SetBool(IsDead, true);
        StartCoroutine(DeSpawn());
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
    }
}
