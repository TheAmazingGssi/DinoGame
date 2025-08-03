using UnityEngine;

public class KnockbackManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private AnimationController animationController;
    private bool isKnockedBack;
    public bool IsKnockedBack => isKnockedBack;

    private EnemyCombatManager combatManager;
    private AnimationManager animManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
        combatManager = GetComponent<EnemyCombatManager>();
        animManager = GetComponent<AnimationManager>();
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        if (combatManager != null) combatManager.IsKnockbacked = true;

        if (animationController != null)
            animationController.TriggerKnockback();

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(force, ForceMode2D.Impulse);

        Invoke(nameof(ResetKnockback), duration);
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
        
        if (combatManager != null) combatManager.IsKnockbacked = false;

        rb.linearVelocity = Vector2.zero; // fixed from linearVelocity

        if (animManager != null)
            animManager.KnockbackEnd();
    }

    public void ForceEndKnockback()
    {
        CancelInvoke(nameof(ResetKnockback));
        ResetKnockback();
    }
}