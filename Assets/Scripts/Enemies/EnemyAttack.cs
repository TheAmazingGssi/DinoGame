using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class EnemyAttack : MonoBehaviour
{
    protected const string PLAYER = "Player";
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int AOEAttack = Animator.StringToHash("AOEAttack");

    [SerializeField] protected EnemyManager manager;
    [SerializeField] private float attackCooldown = 2;
    [SerializeField] private VoiceClips soundEffect;

    private bool isOnCooldown = false;
    private bool isAttacking = false;

    protected abstract EnemyAttackType type { get; }
    protected abstract bool IsPlayerInRange { get; }
    protected virtual float AttackRange => manager.EnemyData.AttackRange;

    public bool IsCurrentlyAttacking => isAttacking;

    public bool CanAttackNow()
    {
        return !isOnCooldown && IsPlayerInRange;
    }
    public void TryAttack()
    {
        if (!isOnCooldown)
        {
            //Debug.Log($"{gameObject.name} starting attack");
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isOnCooldown = true;
        manager.AttackManager.ChangeAttackStatue(true);

        if (type == EnemyAttackType.Melee || type == EnemyAttackType.Ranged)
            manager.Animator.SetTrigger(Attack);
        else if (type == EnemyAttackType.AOE)
            manager.Animator.SetTrigger(AOEAttack);

        ApplyDamage();

        if (soundEffect)
            manager.SoundPlayer.PlaySound(soundEffect);
        else
            manager.SoundPlayer.PlaySound(0);

    }

    protected virtual void ApplyDamage() { }

    public virtual void OnAttackEnd()
    {
        isAttacking = false;
        manager.AttackManager.ChangeAttackStatue(false);
        //Debug.Log($"{gameObject.name} attack ended");

        StartCoroutine(CooldownTimer());
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
        //Debug.Log($"{gameObject.name} cooldown finished");
    }

    public void InterruptAttack()
    {
        if (isAttacking)
        {
            manager.Animator.ResetTrigger(Attack);
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