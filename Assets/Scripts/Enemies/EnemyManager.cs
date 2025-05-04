using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class EnemyManager : MonoBehaviour
{
    private static readonly int IsDead = Animator.StringToHash("Dead");


    [SerializeField] private EnemyCombatManager combatManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Animator animator;

    public Animator Animator => animator;
    public EnemyData EnemyData => enemyData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Rigidbody2D RB => rb; 
    public EnemyController EnemyController => enemyController;
    public EnemyCombatManager CombatManager => combatManager;
    public Transform PlayerTransform => playerTransform;


    public event UnityAction<EnemyManager> OnDeath;

    private void OnEnable()
    {
        combatManager.OnDeath += HandleDeath;
        combatManager.Initialize(enemyData.MaxHealth);
    }

    private void HandleDeath(CombatManager combatManager)
    {
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (!combatManager)
        {
            combatManager = gameObject.GetComponentInChildren<EnemyCombatManager>();
        }
    }

}