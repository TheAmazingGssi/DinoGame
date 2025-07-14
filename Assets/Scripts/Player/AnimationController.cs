using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationData animationData;
    public CharacterType characterType;

    public Animator animator;

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
        // Disabled due to console errors - Maayan
        // animator.SetFloat("AnimationSpeed", speed);
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
        // SetBlockAnimationLoop(!isBlocking); // Disabled as Block holds last frame naturally
    }

    public void SetDowned(bool isDowned)
    {
        animator.SetBool("IsDowned", isDowned);
    }

    public void SetRevived()
    {
        animator.SetTrigger("Revive");
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
}