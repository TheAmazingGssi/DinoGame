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
    public UnityEvent onReviveBack = new UnityEvent();
    public UnityEvent onReviveFront = new UnityEvent();

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animationData == null) Debug.LogError("AnimationData not assigned!");
    }

    private AnimationData.DinosaurAnimationSettings GetSettings()
    {
        return animationData.dinosaurSettings[(int)characterType];
    }

    public void SetMoving(bool isMoving)
    {
        var settings = GetSettings();
        if (settings.useIsMoving) animator.SetBool("IsMoving", isMoving);
    }

    public void SetDowned(bool isDowned)
    {
        var settings = GetSettings();
        if (settings.useIsDowned) animator.SetBool("IsDowned", isDowned);
    }

    public void SetBlocking(bool isBlocking)
    {
        var settings = GetSettings();
        if (settings.useIsBlocking) animator.SetBool("IsBlocking", isBlocking);
    }

    public void SetRevived(bool isRevived)
    {
        var settings = GetSettings();
        if (settings.useIsRevived) animator.SetBool("IsRevived", isRevived);
    }

    public void TriggerAttack()
    {
        var settings = GetSettings();
        animator.SetTrigger("Attack");
        onAttackStart?.Invoke();
        // Adjust animation speed if needed
        animator.speed = settings.attackSpeedMultiplier;
    }

    public void TriggerSpecial()
    {
        var settings = GetSettings();
        animator.SetTrigger("Special");
        onSpecialStart?.Invoke();
        animator.speed = settings.specialSpeedMultiplier;
    }

    public void TriggerKnockback()
    {
        animator.SetTrigger("Knockback");
        onKnockback?.Invoke();
    }

    public void TriggerReviveBack()
    {
        animator.SetTrigger("ReviveBack");
        onReviveBack?.Invoke();
    }

    public void TriggerReviveFront()
    {
        animator.SetTrigger("ReviveFront");
        onReviveFront?.Invoke();
    }

    public void TriggerDowned()
    {
        animator.SetTrigger("Downed");
    }
}