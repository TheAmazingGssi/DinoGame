
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public abstract class EnemyAttack : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    [SerializeField] protected EnemyManager manager;

    private Transform playerTransform;
    private Animator animator;
    private EnemyController movement;
    private EnemyData enemyData;

    private bool isInCooldown = false;
    protected abstract bool IsPlayerInRange { get; }

    private void Awake()
    {
        playerTransform = manager.PlayerTransform.PlayerTransform;
        movement = manager.EnemyController;
        enemyData = manager.EnemyData;
        animator = manager.Animator;
    }

    public void TryAttack()
    {
        if (!isInCooldown)
        {
            StartAttack();
            isInCooldown = true;
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(enemyData.Cooldown);
        isInCooldown = false;

        if (IsPlayerInRange)
        {
            TryAttack();
        }
    }

    protected virtual void StartAttack()
    {
        Debug.Log("Start attack");
        animator.SetTrigger(Attack);
    }

    protected abstract void ApplyDamage();

    public virtual void OnAttackExecute()
    {
        Debug.Log("Attack execute event");
        ApplyDamage();
    }

    public virtual void OnAttackEnd()
    {
        Debug.Log("Attack end event");
        animator.ResetTrigger(Attack);
    }
}