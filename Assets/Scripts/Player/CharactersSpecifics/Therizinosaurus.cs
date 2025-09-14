using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Therizinosaurus : CharacterBase
{
    [SerializeField] private int specialHitCount = 4; 
    [SerializeField] private float specialHitInterval = 0.15f; 
    [SerializeField] private float knockbackMultiplier = 50f; // small knockback strength multiplier

    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
    }
    
public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
{
    if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;

    GameObject activeColliderGO = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
    MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
    var activeCollider = activeColliderGO.GetComponent<Collider2D>();

    // Ensure collider GO starts disabled (no lingering stays)
    if (activeColliderGO.activeSelf) activeColliderGO.SetActive(false);

    // --- VFX ---
    animController.SpecialVfxAnimator.transform.position = activeColliderGO.transform.position + new Vector3(facingRight ? 1.1f : -1.1f, 0f, 0f);
    animController.specialVfxRenderer.flipX = !facingRight;
    animController.SpecialVfxAnimator.SetTrigger("Play");
    
    for (int i = 0; i < specialHitCount; i++)
    {
        activeMeleeDamage.ClearHitList();

        // --- Damage setup for this hit ---
        float perHitDamage = stats.specialAttackDamage / specialHitCount;
        activeMeleeDamage?.PrepareDamage
        (
            perHitDamage,
            false,
            _mainPlayerController.transform,
            _mainPlayerController
        );
        
        // Enable for exactly one physics step so we get exactly one OnTriggerEnter batch.
        activeColliderGO.SetActive(true);
        Physics2D.SyncTransforms();
        yield return new WaitForFixedUpdate();   // <-- only ONE fixed step
        activeColliderGO.SetActive(false);       // close window immediately

        // --- Apply small knockback AFTER closing the window (prevents re-enter) ---
        var circle = GetComponentInChildren<CircleCollider2D>();
        if (circle != null)
        {
            float worldRadius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            var hits = Physics2D.OverlapCircleAll(circle.bounds.center, worldRadius, LayerMask.GetMask("Enemy"));
            foreach (var h in hits)
            {
                if (h.CompareTag("Enemy"))
                {
                    KnockbackHelper.ApplyKnockback(
                        h.transform,
                        transform,
                        KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage * knockbackMultiplier, false)
                    );
                }
            }
        }

        // spacing between multi-hits
        yield return new WaitForSeconds(specialHitInterval);
    }
}

}
