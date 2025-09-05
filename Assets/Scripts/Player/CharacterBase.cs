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
    protected GameObject specialColliderGORight; // For Parasaurolophus
    protected GameObject specialColliderGOLeft; // For Parasaurolophus
    protected AnimationController animController;
    public bool facingRight;
    protected float enableDuration;
    protected float disableDelay;

    protected MeleeDamage rightMeleeDamage;
    protected MeleeDamage leftMeleeDamage;
    protected MeleeDamage specialMeleeDamageRight; // For Parasaurolophus
    protected MeleeDamage specialMeleeDamageLeft; // For Parasaurolophus

    public bool IsAttacking { get; protected set; }

    public virtual void Initialize(CharacterStats.CharacterData characterStats, AnimationController animationController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        _mainPlayerController = GetComponent<MainPlayerController>();
        animController = animationController;
        stats = characterStats;
        facingRight = isFacingRight;
        enableDuration = enable;
        disableDelay = disable;
        IsAttacking = false; 

        rightMeleeColliderGO = rightCollider ?? transform.Find("RightMeleeCollider")?.gameObject;
        leftMeleeColliderGO = leftCollider ?? transform.Find("LeftMeleeCollider")?.gameObject;
        specialColliderGORight = transform.Find("SpecialColliderRight")?.gameObject; // Only for Parasaurolophus
        specialColliderGOLeft = transform.Find("SpecialColliderLeft")?.gameObject; // Only for Parasaurolophus


        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null)
            Debug.LogError($"Failed to find melee colliders on {gameObject.name}!");

        rightMeleeDamage = rightMeleeColliderGO != null ? rightMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        leftMeleeDamage = leftMeleeColliderGO != null ? leftMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        specialMeleeDamageRight = specialColliderGORight != null ? specialColliderGORight.GetComponent<MeleeDamage>() : null;
        specialMeleeDamageLeft = specialColliderGOLeft != null ? specialColliderGOLeft.GetComponent<MeleeDamage>() : null;


        if ((specialColliderGORight != null || specialColliderGOLeft != null) && gameObject.GetComponent<Parasaurolophus>() == null)
            Debug.LogWarning($"SpecialCollider found on {gameObject.name} but not used (only for Parasaurolophus)");
    }
    
    public IEnumerator PerformAttack(float damage, UnityAction<float> onAttack)
    {
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            _mainPlayerController.ToggleIsAttacking();

            float rightColliderPositionX = rightMeleeColliderGO.transform.localPosition.x;
            Transform normalAttackVfxObjectTransform = animController.normalAttackVfxAnimator.transform;

            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
            
            activeMeleeDamage?.PrepareDamage(damage, false, _mainPlayerController.transform, _mainPlayerController);

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

            _mainPlayerController.ToggleIsAttacking();
        }
    }
    public abstract IEnumerator PerformSpecial(UnityAction<float> onSpecial);
}