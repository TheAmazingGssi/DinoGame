using System;
using System.Collections;
using UnityEngine;

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

    public abstract IEnumerator PerformAttack(float damage, int attackCount, Action<float> onAttack);
    public abstract IEnumerator PerformSpecial(Action<float> onSpecial);

    public virtual void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        stats = characterStats;
        facingRight = isFacingRight;
        enableDuration = enable;
        disableDelay = disable;

        // Assign colliders by finding children
        rightMeleeColliderGO = rightCollider ?? transform.Find("RightMeleeCollider")?.gameObject;
        leftMeleeColliderGO = leftCollider ?? transform.Find("LeftMeleeCollider")?.gameObject;
        specialColliderGO = transform.Find("SpecialCollider")?.gameObject; // Only for Parasaurolophus

        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null)
            Debug.LogError($"Failed to find melee colliders on {gameObject.name}!");

        // Cache MeleeDamage components
        rightMeleeDamage = rightMeleeColliderGO != null ? rightMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        leftMeleeDamage = leftMeleeColliderGO != null ? leftMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        specialMeleeDamage = specialColliderGO != null ? specialColliderGO.GetComponent<MeleeDamage>() : null;

        if (specialColliderGO != null && gameObject.GetComponent<Parasaurolophus>() == null)
            Debug.LogWarning($"SpecialCollider found on {gameObject.name} but not used (only for Parasaurolophus)");
    }
}