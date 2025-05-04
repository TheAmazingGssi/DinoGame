using UnityEngine;
using System.Collections;

public abstract class EnemyAttack : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");

    [SerializeField] private EnemyManager manager;

    private Transform playerTransform;
    private Animator animator;
    private EnemyController movement;
    private EnemyData enemyData;

    private bool canAttack = true;


    private void Awake()
    {
        playerTransform = manager.PlayerTransform;
        movement = manager.EnemyController;
        enemyData = manager.EnemyData;
        animator = manager.Animator;
    }
    private void FixedUpdate()
    {
        HandleAttack();
    }

    protected virtual void HandleAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= enemyData.AttackRange && canAttack)
        {
            canAttack = false;
            StartAttack();
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(enemyData.Cooldown);
        canAttack = true;
    }


    protected virtual void StartAttack()
    {
        animator.SetTrigger(Attack);
    }

    protected abstract void ApplyDamage();

    public virtual void OnAttackEnd() //animation event
    {
        Debug.Log("Attackendevent");
        ApplyDamage();
        animator.ResetTrigger(Attack);
    }
}
