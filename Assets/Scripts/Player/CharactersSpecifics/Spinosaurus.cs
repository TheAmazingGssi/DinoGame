using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spinosaurus : CharacterBase
{
    private AnimationController animationController;

    public override void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, rightCollider, leftCollider, isFacingRight, enable, disable);
        animationController = GetComponent<AnimationController>();

        if (animationController == null)
            Debug.LogError($"AnimationController not found in {gameObject.name}!");
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
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null) yield break;

        IsPerformingSpecialMovement = true;
        animationController.TriggerSpecial();

        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        Vector3 startPos = new Vector3(facingRight ? 0.175f : -0.175f, activeCollider.transform.localPosition.y, activeCollider.transform.localPosition.z);
        Vector3 targetPos = new Vector3(facingRight ? 0.45f : -0.45f, startPos.y, startPos.z);
        Transform enemyTransform = null;

        // Wait 0.5s
        yield return new WaitForSeconds(0.5f);

        // Move collider to target over 0.3s
        activeCollider.SetActive(true);
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.3f;
            activeCollider.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            // Check for enemy hit
            if (enemyTransform == null)
            {
                Vector2 castOrigin = activeCollider.transform.position + (facingRight ? Vector3.right : Vector3.left) * 0.2f;
                Vector2 boxSize = new Vector2(0.6f, 0.6f);
                RaycastHit2D hit = Physics2D.BoxCast(castOrigin, boxSize, 0f, facingRight ? Vector2.right : Vector2.left, 0.2f, LayerMask.GetMask("Enemies"));
                
                // Visualize BoxCast
                DrawBoxCast(castOrigin, boxSize, facingRight ? Vector2.right : Vector2.left, 0.2f, Color.red);

                if (hit.collider != null)
                {
                    Debug.Log($"BoxCast hit: {hit.collider.gameObject.name}, Tag: {hit.collider.tag}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        enemyTransform = hit.collider.transform;
                        activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, transform, null);
                        //KnockbackHelper.ApplyKnockback(enemyTransform, transform, KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage, true), KnockbackType.Grab);
                        Debug.Log($"Grabbed enemy: {enemyTransform.gameObject.name}");
                    }
                }
                else
                {
                    Debug.Log("BoxCast hit nothing");
                }
            }

            // Drag enemy if hit
            if (enemyTransform != null)
                enemyTransform.position = activeCollider.transform.position;

            yield return null;
        }

        // Hold for 0.2s
        activeCollider.transform.localPosition = targetPos;
        if (enemyTransform != null)
            enemyTransform.position = activeCollider.transform.position;
        yield return new WaitForSeconds(0.2f);

        // Move collider post-Hold for 0.2s
        elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.2f;
            activeCollider.transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
            if (enemyTransform != null)
                enemyTransform.position = activeCollider.transform.position;
            yield return null;
        }

        activeCollider.transform.localPosition = startPos;
        activeCollider.SetActive(false);
        if (enemyTransform != null)
            enemyTransform.position = activeCollider.transform.position;

        IsPerformingSpecialMovement = false;
    }

    private void DrawBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance, Color color)
    {
        Vector2 endPoint = origin + direction.normalized * distance;
        Vector2 halfSize = size * 0.5f;

        Vector2[] corners = new Vector2[]
        {
            origin + new Vector2(-halfSize.x, -halfSize.y),
            origin + new Vector2(halfSize.x, -halfSize.y),
            origin + new Vector2(halfSize.x, halfSize.y),
            origin + new Vector2(-halfSize.x, halfSize.y)
        };

        Vector2[] endCorners = new Vector2[]
        {
            endPoint + new Vector2(-halfSize.x, -halfSize.y),
            endPoint + new Vector2(halfSize.x, -halfSize.y),
            endPoint + new Vector2(halfSize.x, halfSize.y),
            endPoint + new Vector2(-halfSize.x, halfSize.y)
        };

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(corners[i], corners[(i + 1) % 4], color, 0.1f);
            Debug.DrawLine(endCorners[i], endCorners[(i + 1) % 4], color, 0.1f);
            Debug.DrawLine(corners[i], endCorners[i], color, 0.1f);
        }
    }
}