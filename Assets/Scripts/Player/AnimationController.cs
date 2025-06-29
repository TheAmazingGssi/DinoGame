using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationData animationData;
    public CharacterType characterType;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat("MoveSpeed", speed);
    }

    public void SetAnimationSpeed(float speed)
    {
        animator.SetFloat("AnimationSpeed", speed);
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
        SetBlockAnimationLoop(!isBlocking); // Ensure Block holds last frame
    }

    public void SetDowned(bool isDowned)
    {
        animator.SetBool("IsDowned", isDowned);
    }

    public void SetRevived(bool isRevived)
    {
        animator.SetBool("IsRevived", isRevived);
    }

    public void TriggerDamaged()
    {
        animator.SetTrigger("Damaged");
    }

    public void TriggerKnockback()
    {
        animator.SetTrigger("Knockback");
    }

    public void TriggerEmote()
    {
        animator.SetTrigger("Emote");
    }

    private void SetBlockAnimationLoop(bool loop)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Block"))
        {
            animator.SetBool("IsBlocking", loop);
        }
    }
}