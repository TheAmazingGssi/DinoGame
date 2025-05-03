using UnityEngine;
using System.Collections;

public abstract class EnemyAttack : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyController movement;

    [SerializeField] private float attackRange = 20;
    [SerializeField] private float attackCooldown;

    private bool canAttack = true;

    private void FixedUpdate()
    {
        HandleAttack();
    }

    protected virtual void HandleAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            canAttack = false;
            StartAttack();
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
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
