using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spinosaurus : CharacterBase
{
    [SerializeField] private GameObject specialSpritesContainer;
    [SerializeField] private SpriteRenderer neckSpriteRenderer;
    [SerializeField] private Transform headTransform;

    private float neckMinWidth;
    private float neckMaxWidth;
    private float chompSpeed;
    private float dragSpeed;
    private AnimationController animationController;
    private SpinosaurusHeadController headController;

    public override void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, GameObject specialCollider, bool isFacingRight, float enable, float disable)
    {
        base.Initialize(characterStats, rightCollider, leftCollider, specialCollider, isFacingRight, enable, disable);
        animationController = GetComponent<AnimationController>();
        headController = GetComponent<SpinosaurusHeadController>();
        neckMinWidth = stats.neckMinWidth;
        neckMaxWidth = stats.neckMaxWidth;
        chompSpeed = stats.chompSpeed;
        dragSpeed = stats.dragSpeed;

        if (specialSpritesContainer == null || neckSpriteRenderer == null || headTransform == null || headController == null)
        {
            Debug.LogError($"SpecialSpritesContainer, Neck SpriteRenderer, Head Transform, or HeadController missing on {gameObject.name}!");
        }
    }

    public override IEnumerator PerformSpecial(UnityAction<float> onSpecial)
    {
        if (rightMeleeColliderGO == null || leftMeleeColliderGO == null || neckSpriteRenderer == null || headController == null || specialSpritesContainer == null)
            yield break;

        // Initialize special attack
        animationController.TriggerSpecial();
        specialSpritesContainer.SetActive(true);
        headController.PlayMouthOpen();
        GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
        Vector3 originalColliderPos = activeCollider.transform.localPosition;
        float originalWidth = neckSpriteRenderer.size.x;
        activeCollider.SetActive(true);

        float maxDistance = neckMaxWidth - neckMinWidth;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Collider2D hitEnemy = null;

        // Extend neck
        hitEnemy = yield return ExtendNeck(activeCollider, direction, maxDistance);

        // Pull enemy if grabbed
        if (hitEnemy != null)
        {
            yield return PullEnemy(hitEnemy, onSpecial);
        }

        // Retract neck
        yield return RetractNeck(activeCollider, originalColliderPos, originalWidth);

        // Finalize
        activeCollider.SetActive(false);
        specialSpritesContainer.SetActive(false);
        animationController.GetComponent<Animator>().SetTrigger("BackToNormal");
    }

    private IEnumerator ExtendNeck(GameObject activeCollider, Vector3 direction, float maxDistance)
    {
        float moved = 0f;
        Collider2D hitEnemy = null;

        while (moved < maxDistance && hitEnemy == null)
        {
            float step = chompSpeed * Time.deltaTime;
            neckSpriteRenderer.size += new Vector2(step, 0);
            activeCollider.transform.localPosition += direction * step;
            headTransform.localPosition = new Vector3(neckSpriteRenderer.size.x, headTransform.localPosition.y, headTransform.localPosition.z);
            moved += step;

            RaycastHit2D hit = Physics2D.BoxCast(activeCollider.transform.position, new Vector2(0.5f, 0.5f), 0f, direction, 0f, LayerMask.GetMask("Enemy"));
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                hitEnemy = hit.collider;
                headController.PlayMouthClose();
            }
            yield return null;
        }
        return hitEnemy;
    }

    private IEnumerator PullEnemy(Collider2D hitEnemy, UnityAction<float> onSpecial)
    {
        Transform enemyTransform = hitEnemy.transform;
        Vector3 targetPos = transform.position;

        while (Vector3.Distance(enemyTransform.position, targetPos) > 0.1f)
        {
            enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, targetPos, dragSpeed * Time.deltaTime);
            yield return null;
        }

        onSpecial?.Invoke(stats.specialAttackDamage);
        KnockbackHelper.ApplyKnockback(enemyTransform, transform, KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage, true), KnockbackType.Grab);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator RetractNeck(GameObject activeCollider, Vector3 originalColliderPos, float originalWidth)
    {
        while (neckSpriteRenderer.size.x > neckMinWidth)
        {
            float step = chompSpeed * Time.deltaTime;
            neckSpriteRenderer.size -= new Vector2(step, 0);
            activeCollider.transform.localPosition = Vector3.MoveTowards(activeCollider.transform.localPosition, originalColliderPos, step);
            headTransform.localPosition = new Vector3(neckSpriteRenderer.size.x, headTransform.localPosition.y, headTransform.localPosition.z);
            yield return null;
        }

        neckSpriteRenderer.size = new Vector2(neckMinWidth, neckSpriteRenderer.size.y);
        activeCollider.transform.localPosition = originalColliderPos;
    }
}