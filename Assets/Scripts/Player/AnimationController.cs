using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationData animationData;
    public CharacterType characterType;

    [FormerlySerializedAs("animator")] public Animator mainAnimator;
    public GameObject SpecialVfxObject;
    public Animator SpecialVfxAnimator;
    public Animator haloVfxAnimator;
    public SpriteRenderer specialVfxRenderer;
    public Animator normalAttackVfxAnimator;
    public SpriteRenderer normalAttackVfxRenderer;
    public ParticleSystem terryParticleSystem;
    public ParticleSystem healParticleSystem;
    public RoarWaveBurstSpawner parisRoarWaveSpawner;
    
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
    
    public void SetFrozen(bool isFrozen)
    {
        mainAnimator.SetBool("IsFrozen", isFrozen);
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

    public void TriggerRevive()
    {
        mainAnimator.SetTrigger("Revive");
    }

    public void TriggerDamaged()
    {
        mainAnimator.SetTrigger("Damaged");
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
        mainAnimator.SetTrigger("Knockback");
    }
    
    public void TriggerHalo()
    {
        haloVfxAnimator.SetTrigger("Play");
    }

    public Animator GetAnimator()
    {
        return mainAnimator;
    }
}