using System;
using System.Collections;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected CharacterStats.CharacterData stats;
    protected GameObject rightMeleeColliderGO;
    protected GameObject leftMeleeColliderGO;
    protected bool facingRight;
    protected float enableDuration;
    protected float disableDelay;

    public void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        stats = characterStats;
        rightMeleeColliderGO = rightCollider;
        leftMeleeColliderGO = leftCollider;
        facingRight = isFacingRight;
        enableDuration = enable;
        disableDelay = disable;
    }

    public bool CanPerformSpecial()
    {
        return stats.currentStamina >= stats.specialAttackCost;
    }

    public void ConsumeSpecialStamina()
    {
        stats.currentStamina -= stats.specialAttackCost;
    }

    public abstract IEnumerator PerformAttack(float damage, int attackCount, Action<float> onAttack);

    public abstract IEnumerator PerformSpecial(Action<float> onSpecial);
}