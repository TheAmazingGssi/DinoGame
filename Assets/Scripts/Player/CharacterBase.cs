using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class CharacterBase : MonoBehaviour
{
    protected MainPlayerController _mainPlayerController;
    protected CharacterStats.CharacterData stats;
    protected GameObject rightMeleeColliderGO;
    protected GameObject leftMeleeColliderGO;
    protected GameObject specialColliderGO; // For Parasaurolophus
    protected AnimationController animController;
    public bool facingRight;
    protected float enableDuration;
    protected float disableDelay;

    protected MeleeDamage rightMeleeDamage;
    protected MeleeDamage leftMeleeDamage;
    protected MeleeDamage specialMeleeDamage; // For Parasaurolophus
    public bool IsAttacking { get; protected set; }

    public virtual void Initialize(CharacterStats.CharacterData characterStats, AnimationController animationController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        _mainPlayerController = GetComponent<MainPlayerController>();
        animController = animationController;
        stats = characterStats;
        facingRight = isFacingRight;
        enableDuration = enable;
        disableDelay = disable;
        IsAttacking = false; // Initialize

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
    
    public IEnumerator PerformAttack(float damage, UnityAction<float> onAttack)
    {
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            _mainPlayerController.ToggleIsAttacking();
            //IsAttacking = true;
            
            float rightColliderPositionX = rightMeleeColliderGO.transform.localPosition.x;
            Transform normalAttackVfxObjectTransform = animController.normalAttackVfxAnimator.transform;
            
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
            
            animController.normalAttackVfxAnimator.transform.localPosition = new Vector3
            (
                facingRight ? rightColliderPositionX : (-1) * rightColliderPositionX,
                normalAttackVfxObjectTransform.localPosition.y,
                normalAttackVfxObjectTransform.localPosition.z
            );
            
            animController.normalAttackVfxRenderer.flipX = !facingRight;
            animController.normalAttackVfxAnimator.SetTrigger("Play");
            
            activeCollider.SetActive(true);
            onAttack?.Invoke(damage);
            
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
            yield return new WaitForSeconds(disableDelay);
            
            //IsAttacking = false;
            _mainPlayerController.ToggleIsAttacking();
        }
    }
    public abstract IEnumerator PerformSpecial(UnityAction<float> onSpecial);
}