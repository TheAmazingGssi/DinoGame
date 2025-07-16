using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Triceratops : CharacterBase
{
    [SerializeField] private float chargeDistance = 5f; 
    [SerializeField] private float chargeSpeed = 7f;
    [SerializeField] private float chargeDamageDelay = 0.2f;
    [SerializeField] private float glideDistance = 0.5f;
    [SerializeField] private float glideDuration = 0.2f;

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

        IsPerformingSpecialMovement = true; // Block movement
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeCollider.SetActive(true);

        Vector3 startPos = transform.position;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Vector3 targetPos = startPos + direction * chargeDistance;
        float elapsed = 0f;

        Debug.Log($"Charging in direction {direction}");

        while (elapsed < chargeDistance / chargeSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / (chargeDistance / chargeSpeed));
            elapsed += Time.deltaTime;
            
            if (elapsed >= chargeDamageDelay)
            {
                activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, transform, null);
            }
            yield return null;
        }

        transform.position = targetPos;

        elapsed = 0f;
        startPos = transform.position;
        targetPos = startPos + direction * glideDistance;

        while (elapsed < glideDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / glideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        activeCollider.SetActive(false);
        IsPerformingSpecialMovement = false; // Resume movement
    }
}