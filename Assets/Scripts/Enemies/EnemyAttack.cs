using System.Collections;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    protected static readonly int Attack = Animator.StringToHash("Attack");
    [SerializeField] protected EnemyManager manager;
    protected Animator animator;
    protected EnemyData enemyData;
    protected bool isInCooldown = false;

    protected abstract bool IsPlayerInRange { get; }
    protected abstract float AttackRange { get; }

    private void Awake()
    {
        animator = manager.Animator;
        enemyData = manager.EnemyData;
    }

    public void TryAttack()
    {
        if (!isInCooldown && HasValidTarget())
        {
            StartAttack();
            isInCooldown = true;
            StartCoroutine(CooldownRoutine());
        }
    }

    private bool HasValidTarget()
    {
        return manager.CurrentTarget != null && IsPlayerInRange;
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(enemyData.Cooldown);
        isInCooldown = false;

        if (HasValidTarget())
        {
            TryAttack();
        }
    }

    protected virtual void StartAttack()
    {
        Debug.Log($"Attacking {manager.CurrentTarget.name}");
        animator.SetTrigger(Attack);
    }

    protected abstract void ApplyDamage();

    public virtual void OnAttackExecute()
    {
        ApplyDamage();
    }

    public virtual void OnAttackEnd()
    {
        animator.ResetTrigger(Attack);
    }
    protected bool IsTargetInRange(float range)
    {
        if (manager.CurrentTarget == null) return false;

        float distance = Vector2.Distance(transform.position, manager.CurrentTarget.position);
        return distance <= range;
    }
}