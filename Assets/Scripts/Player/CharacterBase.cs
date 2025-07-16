using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class CharacterBase : MonoBehaviour
{
    protected CharacterStats.CharacterData stats;
    protected GameObject rightMeleeColliderGO;
    protected GameObject leftMeleeColliderGO;
    protected GameObject specialColliderGO; // For Parasaurolophus
    public bool facingRight;
    protected float enableDuration;
    protected float disableDelay;

    protected MeleeDamage rightMeleeDamage;
    protected MeleeDamage leftMeleeDamage;
    protected MeleeDamage specialMeleeDamage; // For Parasaurolophus
    public bool IsPerformingSpecialMovement { get; protected set; } // Added property

    public virtual void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        stats = characterStats;
        facingRight = isFacingRight;
        enableDuration = enable;
        disableDelay = disable;
        IsPerformingSpecialMovement = false; // Initialize

        rightMeleeColliderGO = rightCollider ?? transform.Find("RightMeleeCollider")?.gameObject;
        leftMeleeColliderGO = leftCollider ?? transform.Find("LeftMeleeCollider")?.gameObject;
        specialColliderGO = transform.Find("SpecialCollider")?.gameObject; // Only for Parasaurolophus

        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null)
            Debug.LogError($"Failed to find melee colliders on {gameObject.name}!");

        rightMeleeDamage = rightMeleeColliderGO != null ? rightMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        leftMeleeDamage = leftMeleeColliderGO != null ? leftMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        specialMeleeDamage = specialColliderGO != null ? specialColliderGO.GetComponent<MeleeDamage>() : null;

        if (specialColliderGO != null && gameObject.GetComponent<Parasaurolophus>() == null)
            Debug.LogWarning($"SpecialCollider found on {gameObject.name} but not used (only for Parasaurolophus)");
    }

    public abstract IEnumerator PerformAttack(float damage, UnityAction<float> onAttack);
    public abstract IEnumerator PerformSpecial(UnityAction<float> onSpecial);
}