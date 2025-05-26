using System.Collections;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    protected CharacterStats.CharacterData stats;
    protected GameObject rightMeleeColliderGO;
    protected GameObject leftMeleeColliderGO;
    protected bool facingRight;
    protected float enableDuration;
    protected float disableDelay;

    public void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enableDur, float disableDel)
    {
        stats = characterStats;
        rightMeleeColliderGO = rightCollider;
        leftMeleeColliderGO = leftCollider;
        facingRight = isFacingRight;
        enableDuration = enableDur;
        disableDelay = disableDel;
    }

    public bool CanPerformSpecial()
    {
        return stats.currentStamina >= stats.specialAttackCost;
    }

    public void ConsumeSpecialStamina()
    {
        stats.currentStamina = Mathf.Max(0, stats.currentStamina - stats.specialAttackCost);
    }

    public abstract IEnumerator PerformAttack(float damage, int attackCount, System.Action<float> onAttack);
    public abstract IEnumerator PerformSpecial(System.Action<float> onSpecial);
}