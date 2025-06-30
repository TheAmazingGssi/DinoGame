using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    private static readonly int IsDead = Animator.StringToHash("Dead");

    [Header("Components")]
    [SerializeField] private EnemyAttackManager attackManager;
    [SerializeField] private EnemyCombatManager combatManager;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Drops")]
    [SerializeField] private GameObject healthItem;
    [SerializeField] private float healthItemDropChance = 0.25f;
    [Tooltip("Put -1 for number of players")]
    [SerializeField] private float healthDropAmount;

    private PlayerCombatManager playerCombatManager;

    // Properties
    public EnemyAttackManager AttackManager => attackManager;
    public PlayerCombatManager PlayerCombatManager => playerCombatManager;
    public Animator Animator => animator;
    public EnemyData EnemyData => enemyData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D RB => rb;
    public EnemyController EnemyController => enemyController;
    public EnemyCombatManager CombatManager => combatManager;

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
        combatManager.Initialize(enemyData.MaxHealth);
    }

    #region Death Handling
    private void HandleDeath(CombatManager combatManager)
    {
        Debug.Log("Ded");
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
    }
}