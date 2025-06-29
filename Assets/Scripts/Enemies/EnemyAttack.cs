using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    protected const string PLAYER = "Player";
    protected static readonly int Attack = Animator.StringToHash("Attack");
    [SerializeField] protected EnemyManager manager;
    protected Animator animator;
    protected EnemyData enemyData;

    private bool isOnCooldown = false;
    private bool isCurrentlyAttacking = false;

    protected abstract bool IsPlayerInRange { get; }
    protected virtual float AttackRange => manager.EnemyData.AttackRange;

    private void Awake()
    {
        animator = manager.Animator;
        enemyData = manager.EnemyData;
    }

    public void TryAttack()
    {
        if (!isOnCooldown && !isCurrentlyAttacking && HasValidTarget())
        {
            Debug.Log($"{gameObject.name} starting attack");
            StartAttack();
        }
    }

    private bool HasValidTarget()
    {
        return manager.CurrentTarget != null && IsPlayerInRange;
    }

    private void StartAttack()
    {
        isCurrentlyAttacking = true;
        isOnCooldown = true;

        animator.SetTrigger(Attack);
        ApplyDamage();

        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(enemyData.Cooldown);
        isOnCooldown = false;
        Debug.Log($"{gameObject.name} cooldown finished");
    }

    protected abstract void ApplyDamage();

    public virtual void OnAttackEnd()
    {
        animator.ResetTrigger(Attack);
        isCurrentlyAttacking = false;
        Debug.Log($"{gameObject.name} attack ended");
    }

    public void InterruptAttack()
    {
        if (isCurrentlyAttacking)
        {
            animator.ResetTrigger(Attack);
            animator.Play("Idle", 0, 0f);
            isCurrentlyAttacking = false;
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