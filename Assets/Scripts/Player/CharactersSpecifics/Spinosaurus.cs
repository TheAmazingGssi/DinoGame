using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spinosaurus : CharacterBase
{
    private Transform _specialVfxTransform;
    private static float _specialVfxPositionX = 0.515f;
    private bool specialVfxPerformed = false;
    private bool specialInProgress = false;
   
    public override void Initialize(CharacterStats.CharacterData characterStats, AnimationController animController, GameObject rightCollider, GameObject leftCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, animController, rightCollider, leftCollider, isFacingRight, enable, disable);
        _specialVfxTransform = animController.specialVfxObject.transform;
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        _specialVfxPositionX = facingRight ? 0.515f : -0.515f;
        _specialVfxTransform.localRotation = facingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        _specialVfxTransform.localPosition = new Vector3(_specialVfxPositionX, _specialVfxTransform.localPosition.y, _specialVfxTransform.localPosition.z);
        
        if (specialInProgress) yield break; 
        specialInProgress = true;
        specialVfxPerformed = false;

        animController.TriggerSpecial();

        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        activeMeleeDamage?.PrepareDamage(stats.specialAttackDamage, true, _mainPlayerController.transform, _mainPlayerController);
        Vector3 startPos = new Vector3(facingRight ? 0.175f : -0.175f, activeCollider.transform.localPosition.y, activeCollider.transform.localPosition.z);
        Vector3 targetPos = new Vector3(facingRight ? 0.45f : -0.45f, startPos.y, startPos.z);
        
       
        Transform enemyTransform = null;

        yield return new WaitForSeconds(0.5f);

        // === Attack move phase ===
        activeCollider.SetActive(true);
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.3f;
            activeCollider.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            if (enemyTransform == null)
            {
                Vector2 castOrigin = activeCollider.transform.position + (facingRight ? Vector3.right : Vector3.left) * 0.2f;
                Vector2 boxSize = new Vector2(0.6f, 0.6f);
                RaycastHit2D[] hits = Physics2D.BoxCastAll(
                    castOrigin,
                    boxSize,
                    0f,
                    facingRight ? Vector2.right : Vector2.left,
                    0.2f,
                    LayerMask.GetMask("Combat")
                );

                //DrawBoxCast(castOrigin, boxSize, facingRight ? Vector2.right : Vector2.left, 0.2f, Color.red);

                foreach (var hit in hits)
                {
                    if (hit.collider != null && hit.collider.name == "HurtBox" && hit.collider.tag == "Enemy")
                    {
                        enemyTransform = hit.collider.transform.root;
                        activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, transform, _mainPlayerController);
                        break; 
                    }
                }
            }

            if (enemyTransform != null)
                enemyTransform.position = new Vector3(activeCollider.transform.position.x, enemyTransform.position.y, enemyTransform.position.z);

            yield return null;
        }

        activeCollider.transform.localPosition = targetPos;

        if (!specialVfxPerformed)
        {
            yield return null;
            animController.TriggerSpecialVfx();
            specialVfxPerformed = true;
        }
        
        
        // === Short hold phase ===
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            if (enemyTransform != null)
                enemyTransform.position = new Vector3(activeCollider.transform.position.x, enemyTransform.position.y, enemyTransform.position.z);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.2f);

        //retract
        elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.2f;
            activeCollider.transform.localPosition = Vector3.Lerp(targetPos, startPos, t);

            if (enemyTransform != null)
                enemyTransform.position = new Vector3(activeCollider.transform.position.x, enemyTransform.position.y, enemyTransform.position.z);

            yield return null;
        }

        activeCollider.transform.localPosition = startPos;
        activeCollider.SetActive(false);

        if (enemyTransform != null)
            enemyTransform.position = new Vector3(activeCollider.transform.position.x, enemyTransform.position.y, enemyTransform.position.z);
        
        specialInProgress = false;
        animController.ResetSpecialVfx();
    }
}