using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationData animationData;
    public CharacterType characterType;

    [FormerlySerializedAs("animator")] public Animator mainAnimator;
    public Animator SpecialVfxObject;
    public Animator SpecialVfxAnimator;
    public SpriteRenderer specialVfxRenderer;
    public Animator normalAttackVfxAnimator;
    public SpriteRenderer normalAttackVfxRenderer;
    public ParticleSystem terryParticleSystem;
    
    private bool vfxPlaying = false;
    
    private void Awake()
    {
        specialVfxRenderer = SpecialVfxObject.GetComponent<SpriteRenderer>();
        SpecialVfxAnimator = SpecialVfxObject.GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        mainAnimator.SetFloat("MoveSpeed", speed);
    }

    public void SetAnimationSpeed(float speed)
    {
        mainAnimator.speed = speed;
    }

    public void TriggerAttack()
    {
        mainAnimator.SetTrigger("Attack");
    }

    public void TriggerSpecial()
    {
        mainAnimator.SetTrigger("Special");
    }

    public void SetBlocking(bool isBlocking)
    {
        mainAnimator.SetBool("IsBlocking", isBlocking);
    }

    public void SetEmoting(bool isEmoting)
    {
        mainAnimator.SetBool("IsEmoting", isEmoting);
    }

    public void SetDowned(bool isDowned)
    {
        mainAnimator.SetBool("IsDowned", isDowned);
        mainAnimator.SetTrigger("Downed");
    }

    public void SetRevive()
    {
        mainAnimator.SetTrigger("Revive");
    }

    public void TriggerDamaged()
    {
        mainAnimator.SetTrigger("Damaged");
    }
    
    public void TriggerSpecialVfx()
    {
        if (vfxPlaying) return; // ✅ Ignore extra calls until reset
        vfxPlaying = true;

        if (!SpecialVfxAnimator.gameObject.activeSelf)
            SpecialVfxAnimator.gameObject.SetActive(true);

        SpecialVfxAnimator.SetTrigger("Play");
    }

    public void ResetSpecialVfx()
    {
        vfxPlaying = false; // ✅ Allow next attack to play VFX again
    }


    public void TriggerKnockback()
    {
        mainAnimator.SetTrigger("Knockback");
    }

    public Animator GetAnimator()
    {
        return mainAnimator;
    }
}