using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationData animationData;
    [SerializeField] public CharacterType characterType;

    public UnityEvent onAttackStart = new UnityEvent();
    public UnityEvent onSpecialStart = new UnityEvent();
    public UnityEvent onKnockback = new UnityEvent();
    public UnityEvent onRevive = new UnityEvent();

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animationData == null) Debug.LogError("AnimationData not assigned!");
    }

    private AnimationData.DinosaurAnimationSettings GetSettings()
    {
        return animationData.dinosaurSettings[(int)characterType];
    }

    // Movement / Locomotion
    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat("MoveSpeed", Mathf.Abs(speed));
    }

    // Blocking (combat)
    public void SetBlocking(bool isBlocking)
    {
        if (GetSettings().useIsBlocking)
            animator.SetBool("IsBlocking", isBlocking);
    }

    // Downed (status)
    public void SetDowned(bool isDowned)
    {
        if (GetSettings().useIsDowned)
            animator.SetBool("IsDowned", isDowned);
    }

    // Revived (status)
    public void SetRevived(bool isRevived)
    {
        if (GetSettings().useIsRevived)
            animator.SetBool("IsRevived", isRevived);
    }

    // Attack trigger
    public void TriggerAttack()
    {
        var settings = GetSettings();
        animator.speed = settings.attackSpeedMultiplier;
        animator.SetTrigger("AttackTrigger");
        onAttackStart?.Invoke();
    }

    // Special trigger
    public void TriggerSpecial()
    {
        var settings = GetSettings();
        animator.speed = settings.specialSpeedMultiplier;
        animator.SetTrigger("SpecialTrigger");
        onSpecialStart?.Invoke();
    }

    // Reset animator speed (use with animation events)
    public void ResetSpeed()
    {
        animator.speed = 1f;
    }

    // Knockback (reaction)
    public void TriggerKnockback()
    {
        animator.SetTrigger("KnockbackTrigger");
        onKnockback?.Invoke();
    }

    // Damage (reaction)
    public void TriggerDamage()
    {
        animator.SetBool("IsDamaged", true);
    }

    public void ClearDamage()
    {
        animator.SetBool("IsDamaged", false);
    }

    // Revive (combined front/back)
    public void TriggerRevive()
    {
        animator.SetTrigger("ReviveTrigger");
        onRevive?.Invoke();
    }

    // Emote
    public void TriggerEmote()
    {
        animator.SetTrigger("EmoteTrigger");
    }
    
    public void SetAnimationSpeed(float speed)
    {
        animator.speed = speed;
    }
    
}
