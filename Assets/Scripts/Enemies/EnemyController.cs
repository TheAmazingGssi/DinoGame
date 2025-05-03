using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float detectionRange = 7.0f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;

    private bool isOverGround = true;

    private Vector3 moveDirection = Vector3.zero;
    private bool isFacingLeft = false;

    [SerializeField] private Collider2D groundCheckCollider;

    private void Start()
    {
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        StayInBounds();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    private void Movement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange && isOverGround)
        {
            moveDirection = (player.position - transform.position).normalized;
            moveDirection.y = 0;

        }
        else if (distanceToPlayer <= attackRange)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (moveDirection.x > 0 && isFacingLeft)
        {
            FlipSprite(false);
        }
        else if (moveDirection.x < 0 && !isFacingLeft)
        {
            FlipSprite(true);
        }

        if (distanceToPlayer <= attackRange)
        {
            FlipSprite(transform.position.x - player.position.x > 0);
        }
    }

    private void StayInBounds()
    {
        if (isOverGround)
        {
            Vector2 movement = new Vector2(moveDirection.x, moveDirection.z) * moveSpeed;
            rb.linearVelocity = movement;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void FlipSprite(bool facingLeft)
    {
        isFacingLeft = facingLeft;
        spriteRenderer.flipX = facingLeft;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isOverGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isOverGround = false;
        }
    }
}