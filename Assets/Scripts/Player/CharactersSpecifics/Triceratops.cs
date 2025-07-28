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
    [SerializeField] private float glideSpeed = 3f;

    private Rigidbody2D rb;

    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animationController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animationController, rightCollider, leftCollider, isFacingRight, enable, disable);
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError($"Rigidbody2D not found on {gameObject.name}!");
    }

    
    
    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null || rb == null) yield break;

        //IsAttacking = true;
        _mainPlayerController.ToggleIsAttacking();
        animController.terryParticleSystem.Play();
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeCollider.SetActive(true);

        Vector3 startPos = transform.position;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Vector3 targetPos = startPos + direction * chargeDistance;
        float elapsed = 0f;
        float chargeDuration = chargeDistance / chargeSpeed;

        Debug.Log($"Charging in direction {direction}");

        // Charge phase
        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / chargeDuration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);

            if (elapsed >= chargeDamageDelay)
            {
                activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, transform, null);
            }
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);

        // Glide phase
        elapsed = 0f;
        startPos = transform.position;
        targetPos = startPos + direction * glideDistance;

        while (elapsed < glideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / glideDuration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        activeCollider.SetActive(false);
        animController.terryParticleSystem.Stop();
        _mainPlayerController.ToggleIsAttacking();
        //IsAttacking = false;
    }
    
    
   /* public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null || rb == null)
            yield break;

        //IsAttacking = true;
        _mainPlayerController.ToggleIsAttacking();
        animController.terryParticleSystem.Play();
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeCollider.SetActive(true);

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        // ——— CHARGE PHASE ———
        float chargeTime = chargeDistance / chargeSpeed;
        rb.linearVelocity = dir * chargeSpeed;

        // wait until it's time to deal damage
        yield return new WaitForSeconds(chargeDamageDelay);
        activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, transform, null);

        // finish the rest of the charge
        yield return new WaitForSeconds(chargeTime - chargeDamageDelay);
        rb.linearVelocity = Vector2.zero;

        // ——— GLIDE PHASE ———
        float glideTime = glideDistance / glideSpeed;  // or you already have glideDuration
        rb.linearVelocity = dir * glideSpeed;

        yield return new WaitForSeconds(glideTime);
        rb.linearVelocity = Vector2.zero;

        // clean up
        activeCollider.SetActive(false);
        animController.terryParticleSystem.Pause();
        _mainPlayerController.ToggleIsAttacking();
        //IsAttacking = false;
    }*/

}