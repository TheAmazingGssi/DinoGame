using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class CharacterBase : MonoBehaviour
{
    protected CharacterStats.CharacterData stats;
    protected GameObject rightMeleeColliderGO;
    protected GameObject leftMeleeColliderGO;
    protected GameObject specialColliderGO; // Optional for specific characters
    protected MeleeDamage rightMeleeDamage;
    protected MeleeDamage leftMeleeDamage;
    protected MeleeDamage specialMeleeDamage;
    protected bool facingRight;
    protected float enableDuration;
    protected float disableDelay;

    protected virtual bool UsesSpecialCollider => false; // Override in derived classes if needed

    public virtual void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, GameObject specialCollider, bool isFacingRight, float enable, float disable)
    {
        stats = characterStats;
        facingRight = isFacingRight;
        enableDuration = enable;
        disableDelay = disable;

        // Assign colliders
        rightMeleeColliderGO = rightCollider;
        leftMeleeColliderGO = leftCollider;
        specialColliderGO = UsesSpecialCollider ? specialCollider : null;

        // Validate and cache MeleeDamage components
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null)
        {
            Debug.LogError($"Melee colliders missing on {gameObject.name}!");
            return;
        }

        rightMeleeDamage = rightMeleeColliderGO.GetComponent<MeleeDamage>();
        leftMeleeDamage = leftMeleeColliderGO.GetComponent<MeleeDamage>();
        specialMeleeDamage = specialColliderGO?.GetComponent<MeleeDamage>();

        if (UsesSpecialCollider && specialColliderGO == null)
        {
            Debug.LogError($"Special collider required but not assigned on {gameObject.name}!");
        }
        else if (!UsesSpecialCollider && specialColliderGO != null)
        {
            Debug.LogWarning($"Special collider assigned on {gameObject.name} but not used!");
        }
    }

    public virtual IEnumerator PerformAttack(float damage, UnityAction<float> onAttack)
    {
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;

        if (activeCollider == null || activeMeleeDamage == null)
            yield break;

        activeCollider.SetActive(true);
        onAttack?.Invoke(damage);
        yield return new WaitForSeconds(enableDuration);
        activeCollider.SetActive(false);
        yield return new WaitForSeconds(disableDelay);
    }

    public abstract IEnumerator PerformSpecial(UnityAction<float> onSpecial);
}