using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spinosaurus : CharacterBase
{
    private float specialAttackRange; // GDD: 4 units
    private float chompSpeed; // Speed of head movement
    private float meleeRange; // Distance to drag enemy
    private float dragSpeed; // Speed of dragging enemy
    private Transform neckTransform; // Neck sprite object
    private Animator headAnimator; // Head Animator
    private AnimationController animationController; // Main body Animator

    private void Awake()
    {
        animationController = GetComponent<AnimationController>();
        neckTransform = transform.Find("Neck");
        headAnimator = neckTransform?.Find("Head")?.GetComponent<Animator>();
        
        //if (stats != null)
        //{
        specialAttackRange = stats.SPspecialAttackRange; 
        chompSpeed = stats.SPchompSpeed; 
        meleeRange = stats.SPmeleeRange; 
        dragSpeed = stats.SPdragSpeed; 
        //}
        /*else
        {
            // Fallback values if stats is null
            specialAttackRange = 4f;
            chompSpeed = 8f;
            meleeRange = 1f;
            dragSpeed = 5f;
        }*/
        
        if (neckTransform == null || headAnimator == null)
        {
            Debug.LogError("Neck or Head Animator not found in Spinosaurus hierarchy.");
        }
    }

    public override IEnumerator PerformAttack(float damage, UnityAction<float> onAttack)
    {
        if (rightMeleeColliderGO != null && leftMeleeColliderGO != null)
        {
            GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
            MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
            activeCollider.SetActive(true);
            onAttack?.Invoke(damage);
            yield return new WaitForSeconds(enableDuration);
            activeCollider.SetActive(false);
            yield return new WaitForSeconds(disableDelay);
        }
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null || neckTransform == null || headAnimator == null) yield break;

        animationController.TriggerSpecial(); // Trigger headless SpecialAttack animation
        headAnimator.SetTrigger("MouthOpen"); // Start mouth-opening animation
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        Vector3 originalNeckScale = neckTransform.localScale;
        Vector3 originalColliderPos = activeCollider.transform.localPosition;
        activeCollider.SetActive(true);

        float moved = 0f;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Collider2D hitEnemy = null;

        // Extend neck
        while (moved < specialAttackRange && hitEnemy == null)
        {
            float step = chompSpeed * Time.deltaTime;
            neckTransform.localScale += new Vector3(step / specialAttackRange, 0, 0); // Stretch neck horizontally
            activeCollider.transform.localPosition += direction * step;
            moved += step;

            RaycastHit2D hit = Physics2D.BoxCast(activeCollider.transform.position, new Vector2(0.5f, 0.5f), 0f, direction, 0f, LayerMask.GetMask("Enemy"));
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                hitEnemy = hit.collider;
                headAnimator.SetTrigger("MouthClose"); // Play mouth-closing animation
                break;
            }
            yield return null;
        }

        // Pull enemy if grabbed
        if (hitEnemy != null)
        {
            Transform enemyTransform = hitEnemy.transform;
            Vector3 targetPos = transform.position + direction * meleeRange;
            while (Vector3.Distance(enemyTransform.position, targetPos) > 0.1f)
            {
                enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, targetPos, dragSpeed * Time.deltaTime);
                yield return null;
            }
            onSpecial?.Invoke(stats.specialAttackDamage);
            KnockbackHelper.ApplyKnockback(enemyTransform, transform, KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage, true), KnockbackType.Grab);
        }

        // Retract neck
        while (neckTransform.localScale.x > originalNeckScale.x)
        {
            neckTransform.localScale -= new Vector3(chompSpeed * Time.deltaTime / specialAttackRange, 0, 0);
            activeCollider.transform.localPosition = Vector3.MoveTowards(activeCollider.transform.localPosition, originalColliderPos, chompSpeed * Time.deltaTime);
            yield return null;
        }

        neckTransform.localScale = originalNeckScale;
        activeCollider.transform.localPosition = originalColliderPos;
        activeCollider.SetActive(false);
        headAnimator.SetTrigger("MouthOpen"); // Reset to open mouth or idle
    }
}