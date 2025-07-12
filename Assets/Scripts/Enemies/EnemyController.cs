using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");

    [SerializeField] EnemyManager manager;

    [SerializeField] private float patrolRange = 3f;
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float waitTimeAtEdge = 2f;
    [SerializeField] private float stuckTimeThreshold = 2f;

    private Vector3 patrolStartPos;
    private int patrolDirection = 1;
    private float waitTimer = 0f;
    private float stuckTimer = 0f;
    private Vector3 lastPosition;
    private bool isWaiting = false;

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

        patrolStartPos = transform.position;
        lastPosition = transform.position;
        FlipSprite(patrolDirection < 0);
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
            animator.SetFloat(Speed, 0f);
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
            Patrol();
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

        animator.SetFloat(Speed, moveDirection.magnitude);
    }

    private void Patrol()
    {
        float distanceFromStart = transform.position.x - patrolStartPos.x;

        if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeThreshold)
            {
                patrolDirection *= -1;
                stuckTimer = 0f;
                isWaiting = true;
                waitTimer = waitTimeAtEdge;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;

        if (isWaiting)
        {
            moveDirection = Vector3.zero;
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                FlipSprite(patrolDirection < 0);
            }
            else
            {
                return;
            }
        }

        if (Mathf.Abs(distanceFromStart) > patrolRange)
        {
            patrolStartPos = transform.position;
            patrolDirection = Random.value < 0.5f ? -1 : 1;
            isWaiting = true;
            waitTimer = waitTimeAtEdge;
            moveDirection = Vector3.zero;
            return;
        }

        moveDirection = new Vector3(patrolDirection, 0f, 0f);
    }

    private void FlipSprite(bool facingLeft)
    {
        isFacingLeft = facingLeft;
        spriteRenderer.flipX = !facingLeft;
    }
}