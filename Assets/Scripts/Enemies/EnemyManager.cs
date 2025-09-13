using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    private static readonly int IS_DEAD = Animator.StringToHash("Dead");

    [Header("Components")]
    [SerializeField] private EnemyAttackManager attackManager;
    [SerializeField] private EnemyCombatManager combatManager;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private KnockbackManager knockbackManager;
    [SerializeField] private GameObject projectileDirection;

    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Drops")]
    [SerializeField] private GameObject healthItem;
    [SerializeField] private float healthItemDropChance = 0.25f;
    [Tooltip("Put -1 for number of players")]
    [SerializeField] private float healthDropAmount;

    private PlayerCombatManager playerCombatManager;
    private bool isDead = false;

    public EnemyAttackManager AttackManager => attackManager;
    public PlayerCombatManager PlayerCombatManager => playerCombatManager;
    public Animator Animator => animator;
    public EnemyData EnemyData => enemyData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D RB => rb;
    public EnemyController EnemyController => enemyController;
    public EnemyCombatManager CombatManager => combatManager;
    public SoundPlayer SoundPlayer => soundPlayer;
    public KnockbackManager KnockbackManager => knockbackManager;
    public GameObject ProjectileDirection => projectileDirection;

    public event UnityAction<EnemyManager> OnDeath;

    private void Start()
    {
        if (healthDropAmount < 0)
        {
            healthDropAmount = PlayerEntity.PlayerList.Count;
        }

        GameManager.Instance.ActiveEnemies.Add(this);

        int enemyCount = 0;
        for(int i = 0; i < GameManager.Instance.ActiveEnemies.Count; i++) 
        {
            enemyCount++;
        }
        Debug.Log(enemyCount);

    }

    private void OnEnable()
    {
        combatManager.OnDeath += HandleDeath;
        combatManager.Initialize(enemyData.MaxHealth);
    }

    #region Death Handling
    private void HandleDeath(CombatManager combatManager)
    {
        if (isDead) return;
        isDead = true;

        GameManager.Instance.ActiveEnemies.Remove(this);

        int enemyCount = 0;
        for (int i = 0; i < GameManager.Instance.ActiveEnemies.Count; i++)
        {
            enemyCount++;
        }
        Debug.Log(enemyCount);

        animator.ResetTrigger("Hurt");

        if (enemyController) enemyController.enabled = false;
        if (attackManager) attackManager.enabled = false;
        if (rb) rb.linearVelocity = Vector2.zero;

        animator.SetTrigger(IS_DEAD);
        OnDeath?.Invoke(this);
        GameManager.Instance.IncrementDeathCount();

        if (Random.value < healthItemDropChance)
        {
            for (int i = 0; i < healthDropAmount; i++)
            {
                Vector2 spawnPosition = new Vector2(1, 1);
                while (spawnPosition.y > -0.226)
                {
                    Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
                    spawnPosition = transform.position + (Vector3)randomOffset;
                }
                Instantiate(healthItem, spawnPosition, Quaternion.identity);
            }
        }
        StartCoroutine(DeSpawn());
    }

    [ContextMenu("DeathDebug")]
    public void DeathTest()
    {
        //animator.SetBool(IS_DEAD, true);
        animator.SetTrigger("DeadT");
    }
    private IEnumerator DeSpawn()
    {
        yield return new WaitForSeconds(2f);
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