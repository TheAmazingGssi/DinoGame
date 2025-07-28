using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private AnimationData animationData;
    public CharacterType characterType;

    public Animator animator;
    public Animator specialVfx;
    public Animator normalAttackVfxAnimator;
    public SpriteRenderer normalAttackVfxRenderer;
    public ParticleSystem terryParticleSystem;

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

    public void SetEmoting(bool isEmoting)
    {
        animator.SetBool("IsEmoting", isEmoting);
    }

    public void SetDowned(bool isDowned)
    {
        animator.SetBool("IsDowned", isDowned);
        animator.SetTrigger("Downed");
    }

    public void SetRevived()
    {
        animator.SetTrigger("Revive");
    }

    public void TriggerDamaged()
    {
        animator.SetTrigger("Damaged");
    }
    
    public void TriggerSpecialVfx()
    {
        if(!specialVfx.gameObject.activeSelf)
        {
            specialVfx.gameObject.SetActive(true);
        }
        else if (specialVfx != null)//Add to other calls in script
        {
            specialVfx.SetTrigger("Play");
        }
    }

    public void TriggerKnockback()
    {
        animator.SetTrigger("Knockback");
    }

    public Animator GetAnimator()
    {
        return animator;
    }
}