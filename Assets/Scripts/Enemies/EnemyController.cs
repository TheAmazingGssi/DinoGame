using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] EnemyManager manager;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;
    private EnemyData enemyData;

    private bool isOverGround = true;

    private Vector3 moveDirection = Vector3.zero;
    private bool isFacingLeft = false;

    [SerializeField] private Collider2D groundCheckCollider;

    private void Awake()
    {
        rb = manager.RB;
        spriteRenderer = manager.SpriteRenderer;
        animator = manager.Animator;
        player = manager.PlayerTransform;
        enemyData = manager.EnemyData;
    }
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

        if (distanceToPlayer <= enemyData.DetectionRange && distanceToPlayer > enemyData.AttackRange && isOverGround)
        {
            moveDirection = (player.position - transform.position).normalized;
        }
        else if (distanceToPlayer <= enemyData.AttackRange)
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

        if (distanceToPlayer <= enemyData.AttackRange)
        {
            FlipSprite(transform.position.x - player.position.x > 0);
        }
    }

    private void StayInBounds()
    {
        if (isOverGround)
        {
            Vector2 movement = new Vector2(moveDirection.x, moveDirection.y) * enemyData.Speed;
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