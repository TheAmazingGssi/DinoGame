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
    [SerializeField] private AnimationClip animation;


    private bool isOnCooldown = false;
    private bool isAttacking = false;

    protected abstract EnemyAttackType type { get; }
    protected abstract bool IsPlayerInRange { get; }
    protected virtual float AttackRange => manager.EnemyData.AttackRange;

    public void TryAttack()
    {
        if (!isOnCooldown)
        {
         //   Debug.Log($"{gameObject.name} starting attack");
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

        float animLength = animation ? animation.length : GetAttackAnimationLength();
        StartCoroutine(AttackDurationCoroutine(animation.length));
        StartCoroutine(CooldownCoroutine());
    }
    private float GetAttackAnimationLength()
    {
        AnimationClip[] clips = manager.Animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == "EnemyMeleeAttack") return clip.length;
            else if (clip.name == "RangedAttack") return clip.length;
            else if (clip.name == "Boss_Attack") return clip.length;
        }
        return 0.5f;
    }
    private IEnumerator AttackDurationCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnAttackEnd();
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
        //Debug.Log($"{gameObject.name} cooldown finished");
    }

    protected virtual void ApplyDamage(){}

    public virtual void OnAttackEnd()
    {
        manager.Animator.ResetTrigger(Attack);
        manager.Animator.ResetTrigger(AOEAttack);
        isAttacking = false;
        manager.AttackManager.ChangeAttackStatue(false);
        //Debug.Log($"{gameObject.name} attack ended");
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