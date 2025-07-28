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

    public void SetRevived()
    {
        mainAnimator.SetTrigger("Revive");
    }

    public void TriggerDamaged()
    {
        mainAnimator.SetTrigger("Damaged");
    }
    
    public void TriggerSpecialVfx()
    {
        if(!SpecialVfxAnimator.gameObject.activeSelf)
        {
            SpecialVfxAnimator.gameObject.SetActive(true);
        }
        else if (SpecialVfxAnimator != null)//Add to other calls in script
        {
            SpecialVfxAnimator.SetTrigger("Play");
        }
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