using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationData animationData;
    public CharacterType characterType;

    private Animator animator;
    private int specialTriggerHash;
    private int spinoSpecialTriggerHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError($"Animator not found on {gameObject.name}!");

        specialTriggerHash = Animator.StringToHash("Special");
        spinoSpecialTriggerHash = Animator.StringToHash("SpinoSpecial");
    }

    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat("MoveSpeed", speed);
    }

    public void SetAnimationSpeed(float speed)
    {
        // Disabled due to console errors - Maayan
        // animator.SetFloat("AnimationSpeed", speed);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
        LogAnimation("Attack");
    }

    public void TriggerSpecial()
    {
        int index = (int)characterType;
        bool useSpinoSpecial = animationData.dinosaurSettings[index].useSpinoSpecial;
        animator.SetTrigger(useSpinoSpecial ? spinoSpecialTriggerHash : specialTriggerHash);
        LogAnimation(useSpinoSpecial ? "SpinoSpecial" : "Special");
    }

    public void TriggerSpinoSpecial()
    {
        animator.SetTrigger(spinoSpecialTriggerHash);
        LogAnimation("SpinoSpecial");
    }

    public void SetBlocking(bool isBlocking)
    {
        animator.SetBool("IsBlocking", isBlocking);
        LogAnimation($"IsBlocking set to {isBlocking}");
    }

    public void SetDowned(bool isDowned)
    {
        animator.SetBool("IsDowned", isDowned);
        LogAnimation($"IsDowned set to {isDowned}");
    }

    public void SetRevived()
    {
        animator.SetTrigger("Revive");
        LogAnimation("Revive");
    }

    public void TriggerDamaged()
    {
        animator.SetTrigger("Damaged");
        LogAnimation("Damaged");
    }

    public void TriggerKnockback()
    {
        animator.SetTrigger("Knockback");
        LogAnimation("Knockback");
    }

    public void SetEmote(bool isEmoting)
    {
        animator.SetBool("IsEmoting", isEmoting);
        LogAnimation($"IsEmoting set to {isEmoting}");
    }

    private void LogAnimation(string message)
    {
        Debug.Log($"Animation: {message} on {gameObject.name}");
    }
}