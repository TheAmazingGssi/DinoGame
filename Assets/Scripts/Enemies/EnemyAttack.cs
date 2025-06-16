using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    protected const string PLAYER = "Player";
    protected static readonly int Attack = Animator.StringToHash("Attack");

    [SerializeField] protected EnemyManager manager;
    [SerializeField] protected float attackRange = 2f;

    protected Animator animator;
    protected EnemyData enemyData;
    protected bool isInCooldown = false;
    private bool isCurrentlyAttacking = false;

    protected abstract bool IsPlayerInRange { get; }
    protected virtual float AttackRange => attackRange;

    private void Awake()
    {
        animator = manager.Animator;
        enemyData = manager.EnemyData;
    }

    public void TryAttack()
    {
        if (!isInCooldown && !manager.IsAttacking && HasValidTarget())
        {
            StartAttack();
            isInCooldown = true;
            isCurrentlyAttacking = true;
            manager.SetAttackState(true, this);
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
        animator.SetTrigger(Attack);
        ApplyDamage();
    }

    protected abstract void ApplyDamage();

    public virtual void OnAttackExecute()
    {
    }

    public virtual void OnAttackEnd()
    {
        animator.ResetTrigger(Attack);
        isCurrentlyAttacking = false;
        manager.SetAttackState(false);
    }

    public void InterruptAttack()
    {
        if (isCurrentlyAttacking)
        {
            animator.ResetTrigger(Attack);
            animator.Play("Idle", 0, 0f);

            isCurrentlyAttacking = false;
            manager.SetAttackState(false);

            Debug.Log($"Attack interrupted on {gameObject.name}");
        }
    }

    protected bool IsTargetInRange(float range)
    {
        if (manager.CurrentTarget == null) return false;
        float distance = Vector2.Distance(transform.position, manager.CurrentTarget.position);
        return distance <= range;
    }

    protected List<PlayerCombatManager> GetPlayersInRange(float range)
    {
        List<PlayerCombatManager> playersInRange = new List<PlayerCombatManager>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(PLAYER))
            {
                PlayerCombatManager player = collider.GetComponent<PlayerCombatManager>();
                if (player != null)
                {
                    playersInRange.Add(player);
                }
            }
        }
        return playersInRange;
    }
}