using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spinosaurus : CharacterBase
{
    [SerializeField] private float specialAttackRange = 4f; // GDD: 4 units
    [SerializeField] private float chompSpeed = 8f; // Speed of head movement
    [SerializeField] private float meleeRange = 1f; // Distance to drag enemy
    [SerializeField] private float dragSpeed = 5f; // Speed of dragging enemy

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
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;

        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        Vector3 originalPos = activeCollider.transform.localPosition;
        activeCollider.SetActive(true);

        float moved = 0f;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Collider2D hitEnemy = null;

        while (moved < specialAttackRange && hitEnemy == null)
        {
            float step = chompSpeed * Time.deltaTime;
            activeCollider.transform.localPosition += direction * step;
            moved += step;

            Collider2D[] hits = Physics2D.OverlapCircleAll(activeCollider.transform.position, 1f, LayerMask.GetMask("Enemy"));
            hitEnemy = System.Array.Find(hits, h => h.CompareTag("Enemy"));
            if (hitEnemy != null) break;

            yield return null;
        }

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

        while (Vector3.Distance(activeCollider.transform.localPosition, originalPos) > 0.1f)
        {
            activeCollider.transform.localPosition = Vector3.MoveTowards(activeCollider.transform.localPosition, originalPos, chompSpeed * Time.deltaTime);
            yield return null;
        }

        activeCollider.SetActive(false);
        activeCollider.transform.localPosition = originalPos;
    }
}