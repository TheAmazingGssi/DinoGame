using UnityEngine;
using System.Collections;

public abstract class EnemyAttack : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");

    [SerializeField] protected EnemyManager manager;

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
        if (canAttack)
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
        ApplyDamage();
        animator.ResetTrigger(Attack);
    }

    protected abstract void ApplyDamage();

    public virtual void OnAttackEnd() //animation event
    {
        Debug.Log("Attackendevent");
        ApplyDamage();
        animator.ResetTrigger(Attack);
    }
}
