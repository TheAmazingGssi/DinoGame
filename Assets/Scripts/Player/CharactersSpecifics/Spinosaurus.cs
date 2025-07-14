using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spinosaurus : CharacterBase
{
    private Animator headAnimator;
    private AnimationController animationController;
    private SpinosaurusNeckController neckController;

    public override void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, rightCollider, leftCollider, isFacingRight, enable, disable);
        animationController = GetComponent<AnimationController>();
        neckController = GetComponent<SpinosaurusNeckController>();
        headAnimator = transform.Find("SpecialSpritesContainer/Neck/Head")?.GetComponent<Animator>();

        if (neckController == null || headAnimator == null)
            Debug.LogError($"SpinosaurusNeckController or Head Animator not found in {gameObject.name}!");

        neckController.Initialize(characterStats, rightCollider, leftCollider, isFacingRight);
    }

    public override IEnumerator PerformAttack(float damage, UnityAction<float> onAttack)
    {
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
            activeCollider.SetActive(true);
            onAttack?.Invoke(damage);
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
            yield return new WaitForSeconds(disableDelay);
        }
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null || headAnimator == null || neckController == null) yield break;

        IsPerformingSpecialMovement = true;
        animationController.TriggerSpecial();
        int mouthOpenHash = Animator.StringToHash("MouthOpen");
        headAnimator.Play(mouthOpenHash, 0, 0f);

        // Wait for neck extension (animation-driven)
        yield return new WaitUntil(() => neckController != null);

        // Hold MouthOpen at last frame if no enemy hit
        if (headAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == mouthOpenHash)
            headAnimator.Play(mouthOpenHash, 0, 1f);

        headAnimator.SetTrigger("MouthClose");

        // Wait for neck retraction
        yield return new WaitUntil(() => neckController != null);

        if (headAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("MouthClose"))
            headAnimator.Play("MouthClose", 0, 0f);

        IsPerformingSpecialMovement = false;
    }

    public void PauseBodyAnimation()
    {
        animationController.animator.speed = 0f; // Pause body animation
        neckController.OnNeckRetractionComplete += ResumeBodyAnimation; // Subscribe to resume
    }

    private void ResumeBodyAnimation()
    {
        animationController.animator.speed = 1f; // Resume body animation
        neckController.OnNeckRetractionComplete -= ResumeBodyAnimation; // Unsubscribe
    }
}