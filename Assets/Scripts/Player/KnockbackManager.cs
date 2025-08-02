using UnityEngine;

public class KnockbackManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private AnimationController animationController;
    private bool isKnockedBack;
    public bool IsKnockedBack => isKnockedBack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        isKnockedBack = true;
        if(animationController != null)
            animationController.TriggerKnockback();
        rb.AddForce(force, ForceMode2D.Impulse);
        Invoke(nameof(ResetKnockback), duration);
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }
}