using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    protected const string PLAYER = "Player";
    protected static readonly int Attack = Animator.StringToHash("Attack");

    [SerializeField] protected EnemyManager manager;
    [SerializeField] private float attackCooldown;

    private bool isOnCooldown = false;
    private bool isAttacking = false;

    protected abstract bool IsPlayerInRange { get; }
    protected virtual float AttackRange => manager.EnemyData.AttackRange;

    public void TryAttack()
    {
        if (!isOnCooldown)
        {
            Debug.Log($"{gameObject.name} starting attack");
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isOnCooldown = true;
        manager.AttackManager.ChangeAttackStatue(true);
        manager.Animator.SetTrigger(Attack);
        ApplyDamage();

        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
        Debug.Log($"{gameObject.name} cooldown finished");
    }

    protected virtual void ApplyDamage()
    {
        OnAttackEnd();
    }

    public virtual void OnAttackEnd()
    {
        manager.Animator.ResetTrigger(Attack);
        isAttacking = false;
        manager.AttackManager.ChangeAttackStatue(false);
        Debug.Log($"{gameObject.name} attack ended");
    }

    public void InterruptAttack()
    {
        if (isAttacking)
        {
            manager.Animator.ResetTrigger(Attack);
            manager.Animator.Play("Idle", 0, 0f);
            isAttacking = false;
            Debug.Log($"Attack interrupted on {gameObject.name}");
        }
    }

    protected bool IsTargetInRange(float range)
    {
        if (manager.AttackManager.CurrentTarget == null) return false;
        float distance = Vector2.Distance(transform.position, manager.AttackManager.CurrentTarget.position);
        return distance <= range;
    }

    protected List<PlayerCombatManager> GetPlayersInRange()
    {
        List<PlayerCombatManager> playersInRange = new List<PlayerCombatManager>();
        foreach (PlayerCombatManager playerCombatManager in manager.AttackManager.PlayersInRange)
        {
            float distance = Vector2.Distance(transform.position, playerCombatManager.transform.position);
            if (distance <= AttackRange) playersInRange.Add(playerCombatManager);
        }
        return playersInRange;
    }
}