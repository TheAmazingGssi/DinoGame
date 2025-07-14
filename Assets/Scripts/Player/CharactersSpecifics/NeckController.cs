using System;
using System.Collections;
using UnityEngine;

public class SpinosaurusNeckController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer neckSpriteRenderer;
    [SerializeField] private GameObject specialSpritesContainer;
    [SerializeField] private GameObject rightMeleeColliderGO;
    [SerializeField] private GameObject leftMeleeColliderGO;
    private CharacterStats.CharacterData stats; // Store stats
    private float neckMinWidth;
    private float neckMaxWidth;
    private float chompSpeed;
    private float dragSpeed;
    private bool facingRight;
    private GameObject activeCollider;
    private MeleeDamage activeMeleeDamage;
    private Vector3 originalColliderPos;
    private Coroutine neckCoroutine;
    public event Action OnNeckRetractionComplete;

    public void Initialize(CharacterStats.CharacterData characterStats, GameObject rightCollider, GameObject leftCollider, bool isFacingRight)
    {
        stats = characterStats; // Store stats
        neckMinWidth = stats.neckMinWidth;
        neckMaxWidth = stats.neckMaxWidth;
        chompSpeed = stats.chompSpeed;
        dragSpeed = stats.dragSpeed;
        rightMeleeColliderGO = rightCollider;
        leftMeleeColliderGO = leftCollider;
        facingRight = isFacingRight;
        activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
        activeMeleeDamage = activeCollider.GetComponent<MeleeDamage>();
        originalColliderPos = activeCollider.transform.localPosition;

        if (specialSpritesContainer == null || neckSpriteRenderer == null || rightMeleeColliderGO == null || leftMeleeColliderGO == null)
            Debug.LogError($"SpecialSpritesContainer, Neck SpriteRenderer, or Colliders not found in {gameObject.name}!");
    }

    public void StartNeckExtension()
    {
        if (neckCoroutine != null) StopCoroutine(neckCoroutine);
        neckCoroutine = StartCoroutine(ExtendNeck());
    }

    public void StopNeckExtension()
    {
        if (neckCoroutine != null)
        {
            StopCoroutine(neckCoroutine);
            neckCoroutine = null;
        }
    }

    public void StartNeckRetraction()
    {
        if (neckCoroutine != null) StopCoroutine(neckCoroutine);
        neckCoroutine = StartCoroutine(RetractNeck());
    }

    private IEnumerator ExtendNeck()
    {
        specialSpritesContainer.SetActive(true);
        activeCollider.SetActive(true);
        neckSpriteRenderer.size = new Vector2(neckMinWidth, neckSpriteRenderer.size.y);
        float maxDistance = neckMaxWidth - neckMinWidth;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        float moved = 0f;
        Collider2D hitEnemy = null;

        while (moved < maxDistance && hitEnemy == null)
        {
            float step = chompSpeed * Time.deltaTime * (maxDistance / 4f);
            neckSpriteRenderer.size += new Vector2(step, 0);
            activeCollider.transform.localPosition += direction * step;
            moved += step;

            RaycastHit2D hit = Physics2D.BoxCast(activeCollider.transform.position, new Vector2(0.5f, 0.5f), 0f, direction, 0f, LayerMask.GetMask("Enemy"));
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                hitEnemy = hit.collider;
                break;
            }
            yield return null;
        }

        if (hitEnemy != null)
        {
            Transform enemyTransform = hitEnemy.transform;
            Vector3 targetPos = transform.position;
            while (Vector3.Distance(enemyTransform.position, targetPos) > 0.1f)
            {
                enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, targetPos, dragSpeed * Time.deltaTime);
                yield return null;
            }
            activeMeleeDamage?.ApplyDamage(stats.specialAttackDamage, true, transform, null);
            KnockbackHelper.ApplyKnockback(enemyTransform, transform, KnockbackHelper.GetKnockbackForceFromDamage(stats.specialAttackDamage, true), KnockbackType.Grab);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator RetractNeck()
    {
        while (neckSpriteRenderer.size.x > neckMinWidth)
        {
            float step = chompSpeed * Time.deltaTime * ((neckMaxWidth - neckMinWidth) / 4f);
            neckSpriteRenderer.size -= new Vector2(step, 0);
            activeCollider.transform.localPosition = Vector3.MoveTowards(activeCollider.transform.localPosition, originalColliderPos, step);
            yield return null;
        }

        neckSpriteRenderer.size = new Vector2(neckMinWidth, neckSpriteRenderer.size.y);
        activeCollider.transform.localPosition = originalColliderPos;
        activeCollider.SetActive(false);
        specialSpritesContainer.SetActive(false);
        OnNeckRetractionComplete?.Invoke();
    }
}