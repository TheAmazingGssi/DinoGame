using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    // ---------- Inspector: Config ----------
    [Header("Config")]
    [SerializeField] public CharacterType characterType;
    [SerializeField] private AnimationData animationData;

    // ---------- Inspector: Core References ----------
    [Header("Core")]
    [SerializeField] private MainPlayerController playerController;
    [SerializeField] private Animator animator;

    // ---------- Inspector: VFX Objects & Animators ----------
    [Header("VFX Objects & Animators")]
    [SerializeField]
    public GameObject specialVfxObject;
    [SerializeField] public Animator specialVfxAnimator;
    [SerializeField] private Animator haloVfxAnimator;
    [SerializeField] private Animator bloodVfxAnimator;
    [SerializeField] public Animator normalAttackVfxAnimator;

    // ---------- Inspector: Renderers ----------
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer mainSpriteRenderer;
    [SerializeField] public SpriteRenderer specialVfxRenderer;
    [SerializeField] public SpriteRenderer normalAttackVfxRenderer;

    // ---------- Inspector: Particles & Spawners ----------
    [Header("Particles & Spawners")]
    [SerializeField]
    public ParticleSystem terryParticleSystem;
    [SerializeField] private ParticleSystem healParticleSystem;
    [SerializeField] private RoarWaveBurstSpawner parisRoarWaveSpawner;

    // ---------- Inspector: Tuning ----------
    [Header("Tuning")]
    [SerializeField, Min(0f)] private float bloodVfxDuration = 0.125f;

    // ---------- Private State ----------
    private static readonly Color HurtPulseColor = new Color(0.5f, 0f, 0f, 1f);
    private bool vfxPlaying = false;

    
    private void Awake()
    {
        specialVfxRenderer = specialVfxObject.GetComponent<SpriteRenderer>();
        specialVfxAnimator = specialVfxObject.GetComponent<Animator>();
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
        if (animator.GetBool("IsDowned")) 
            return;
        
        if(!playerController.IsFrozen)
            animator.SetTrigger("Damaged");
        
        bloodVfxAnimator.SetTrigger("Play");
        StartCoroutine(PulseRedOnHurt());
    }

    private IEnumerator PulseRedOnHurt()
    {
        mainSpriteRenderer.color = HurtPulseColor;
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

        if (!specialVfxAnimator.gameObject.activeSelf)
            specialVfxAnimator.gameObject.SetActive(true);

        specialVfxAnimator.SetTrigger("Play");
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