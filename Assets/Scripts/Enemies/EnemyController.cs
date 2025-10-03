using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");

    [SerializeField] EnemyManager manager;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private EnemyData enemyData;
    private Transform currentTarget;
    private Vector3 moveDirection = Vector3.zero;
    private bool isFacingLeft = false;

    private void Awake()
    {
        rb = manager.RB;
        spriteRenderer = manager.SpriteRenderer;
        animator = manager.Animator;
        enemyData = manager.EnemyData;
    }

    private void Start()
    {
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        bool shallIFlip = ShouldIFlip();
        
        if (shallIFlip)
            FlipSprite(shallIFlip);
    }
    
    private bool ShouldIFlip()
    {
        foreach (var player in PlayerEntity.PlayerList)
        {
            if(player.transform.position.x < transform.position.x)
                return false;
        }

        if (!spriteRenderer.flipX)
            return true;

        return false;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (manager.AttackManager.IsAttacking || manager.KnockbackManager.IsKnockedBack)
        {
            rb.linearVelocity = Vector2.zero;
            moveDirection = Vector3.zero;
            animator.SetFloat(Speed, rb.linearVelocity.magnitude);
            return;
        }

        currentTarget = manager.AttackManager.CurrentTarget;

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget > enemyData.StopRange)
            {
                moveDirection = (currentTarget.position - transform.position).normalized;
            }
            else
            {
                moveDirection = Vector3.zero;
                bool shouldFaceLeft = transform.position.x > currentTarget.position.x;
                FlipSprite(shouldFaceLeft);
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, PlayerEntity.PlayerList[0].transform.position);

            if (distanceToTarget > enemyData.StopRange)
            {
                moveDirection = (PlayerEntity.PlayerList[0].transform.position - transform.position).normalized;
            }
            else
            {
                moveDirection = Vector3.zero;
                bool shouldFaceLeft = transform.position.x > PlayerEntity.PlayerList[0].transform.position.x;
                FlipSprite(shouldFaceLeft);
            }
        }

        if (moveDirection.x > 0 && isFacingLeft)
        {
            FlipSprite(false);
        }
        else if (moveDirection.x < 0 && !isFacingLeft)
        {
            FlipSprite(true);
        }

        Vector2 movement = new Vector2(moveDirection.x, moveDirection.y) * enemyData.Speed;
        rb.linearVelocity = movement;

        animator.SetFloat(Speed, rb.linearVelocity.magnitude);
    }

    private void FlipSprite(bool facingLeft)
    {
        isFacingLeft = facingLeft;
        spriteRenderer.flipX = !facingLeft;
    }

    public bool IsFacingLeft()
    {
        return isFacingLeft;
    }
}
