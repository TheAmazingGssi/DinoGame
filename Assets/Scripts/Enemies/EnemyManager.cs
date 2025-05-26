using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Linq;


public class EnemyManager : MonoBehaviour
{
    private static readonly int IsDead = Animator.StringToHash("Dead");
    private const string Player = "Player";



    [SerializeField] private EnemyCombatManager combatManager;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private PlayerTransformData playerTransform;

    private List<PlayerCombatManager> playersInRange;

    private Transform currentTarget;

    private PlayerCombatManager playerCombatManager;

    public PlayerCombatManager PlayerCombatManager => playerCombatManager;
    public Transform CurrentTarget => currentTarget;
    public Animator Animator => animator;
    public EnemyData EnemyData => enemyData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D RB => rb; 
    public EnemyController EnemyController => enemyController;
    public EnemyCombatManager CombatManager => combatManager;
    public PlayerTransformData PlayerTransform => playerTransform;

    [SerializeField] private bool isDead = false;


    public event UnityAction<EnemyManager> OnDeath;

    private void OnEnable()
    {
        combatManager.OnDeath += HandleDeath;
        combatManager.Initialize(enemyData.MaxHealth);
    }
    private void Update()
    {
        playersInRange.RemoveAll(player => player == null);


        if (currentTarget == null && playersInRange.Count > 0)
        {
            SelectTarget();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            playersInRange.Add(collision.gameObject.GetComponent<PlayerCombatManager>());

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            Debug.Log("Player left detection range");
            playersInRange.Remove(collision.gameObject.GetComponent<PlayerCombatManager>());
            if (playersInRange.Count <= 0)
            {
                currentTarget = null;
                playerCombatManager = null;
            }
        }
    }
    private void SelectTarget()
    {
        if (playersInRange.Count == 0) return;

        Transform lowestHealthTarget = playersInRange[0].transform;
        float lowestHealth = playersInRange[0].CurrentHealth;
        PlayerCombatManager combatManager = playersInRange[0]; 

        foreach (var player in playersInRange)
        {
            if (player.CurrentHealth < lowestHealth)
            {
                lowestHealth = player.CurrentHealth;
                lowestHealthTarget = player.transform;
                combatManager = player;
            }
        }

        playerCombatManager = combatManager;
        currentTarget = lowestHealthTarget;
    }
    private IEnumerator DeSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);

    }

    private void HandleDeath(CombatManager combatManager)
    {
        OnDeath?.Invoke(this);
        animator.SetBool(IsDead, true);
        StartCoroutine(DeSpawn());
    }

    private void OnValidate()
    {
        if (!combatManager)
        {
            combatManager = gameObject.GetComponentInChildren<EnemyCombatManager>();
        }
    }

}