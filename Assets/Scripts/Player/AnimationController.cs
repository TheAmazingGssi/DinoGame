using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationController : MonoBehaviour
{
    public AnimationData animationData;
    public CharacterType characterType;

    public GameObject SpecialVfxObject;
    public Animator animator;
    public Animator SpecialVfxAnimator;
    public Animator haloVfxAnimator;
    public Animator bloodVfxAnimator;
    public Animator normalAttackVfxAnimator;
    public SpriteRenderer mainSpriteRenderer;
    public SpriteRenderer specialVfxRenderer;
    public SpriteRenderer normalAttackVfxRenderer;
    public ParticleSystem terryParticleSystem;
    public ParticleSystem healParticleSystem;
    public RoarWaveBurstSpawner parisRoarWaveSpawner;
    public float bloodVfxDuration = 0.1f;
    
    private Color hurtPulseColor = new Color(0.5f, 0f, 0f, 1f);
    private bool vfxPlaying = false;
    
    private void Awake()
    {
        specialVfxRenderer = SpecialVfxObject.GetComponent<SpriteRenderer>();
        SpecialVfxAnimator = SpecialVfxObject.GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat("MoveSpeed", speed);
    }

    public void SetAnimationSpeed(float speed)
    {
        animator.speed = speed;
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TriggerSpecial()
    {
        animator.SetTrigger("Special");
    }

    public void SetBlocking(bool isBlocking)
    {
        animator.SetBool("IsBlocking", isBlocking);
    }
    
    public void SetFrozen(bool isFrozen)
    {
        animator.SetBool("IsFrozen", isFrozen);
    }

    public void SetEmoting(bool isEmoting)
    {
        animator.SetBool("IsEmoting", isEmoting);
    }

    public void SetDowned(bool isDowned)
    {
        animator.SetBool("IsDowned", isDowned);
        animator.SetTrigger("Downed");
    }

    public void TriggerRevive()
    {
        animator.SetTrigger("Revive");
    }

    public void TriggerDamaged()
    {
        animator.SetTrigger("Damaged");
        bloodVfxAnimator.SetTrigger("Play");
        StartCoroutine(PulseRedOnHurt());
    }

    private IEnumerator PulseRedOnHurt()
    {
        mainSpriteRenderer.color = hurtPulseColor;
        yield return new WaitForSeconds(bloodVfxDuration);
        mainSpriteRenderer.color = Color.white;
    }
    
    public void TriggerSpecialVfx()
    {
        if (characterType == CharacterType.Parasaurolophus)
        { 
            parisRoarWaveSpawner?.TriggerBurst();
            return; 
        }
        
        if (vfxPlaying) return; 
        vfxPlaying = true;

        if (!SpecialVfxAnimator.gameObject.activeSelf)
            SpecialVfxAnimator.gameObject.SetActive(true);

        SpecialVfxAnimator.SetTrigger("Play");
    }

    public void ResetSpecialVfx()
    {
        vfxPlaying = false; 
    }
    
    public void StartHealVfx()
    {
        if (!healParticleSystem.isPlaying)
            healParticleSystem.Play();
    }
    
    public void StopHealVfx()
    {
        if (healParticleSystem.isPlaying)
            healParticleSystem.Stop();
    }
    
    public void HealBurst()
    {
        if (!healParticleSystem.isPlaying)
            StartCoroutine(HealBurstCoroutine());
    }
    
    private IEnumerator HealBurstCoroutine()
    {
        healParticleSystem.Play();
        yield return new WaitForSeconds(1f);
        healParticleSystem.Stop();
    }
    


    public void TriggerKnockback()
    {
        animator.SetTrigger("Knockback");
    }
    
    public void TriggerHalo()
    {
        haloVfxAnimator.SetTrigger("Play");
    }

    public Animator GetAnimator()
    {
        return animator;
    }
}