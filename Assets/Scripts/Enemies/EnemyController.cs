using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private const string Ground = "Ground";

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
    private bool isOverGround = true;
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
        currentTarget = manager.AttackManager.CurrentTarget;

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget > enemyData.StopRange && isOverGround)
            {
                moveDirection = (currentTarget.position - transform.position).normalized;
            }
            else if (distanceToTarget <= enemyData.StopRange)
            {
                moveDirection = Vector3.zero;
            }
            else
            {
                moveDirection = Vector3.zero;
            }

            if (distanceToTarget <= enemyData.StopRange)
            {
                //FlipSprite(transform.position.x - currentTarget.position.x > 0);
            }
        }
        else
        {
            Patrol();        
        }

        if (moveDirection.x > 0 && isFacingLeft)
        {
            FlipSprite(true);
        }
        else if (moveDirection.x < 0 && !isFacingLeft)
        {
            FlipSprite(false);
        }

        animator.SetFloat(Speed, moveDirection.magnitude);
    }

    private void Patrol()
    {
        float distanceFromStart = transform.position.x - patrolStartPos.x;

        if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
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
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        if (Mathf.Abs(distanceFromStart) >= patrolRange)
        {
            patrolDirection *= -1;
            isWaiting = true;
            waitTimer = waitTimeAtEdge;
            moveDirection = Vector3.zero;
            return;
        }

        moveDirection = new Vector3(patrolDirection, 0f, 0f);

        if (patrolDirection > 0 && isFacingLeft)
        {
            FlipSprite(true);
        }
        else if (patrolDirection < 0 && !isFacingLeft)
        {
            FlipSprite(false);
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

/*    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Ground))
        {
            isOverGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Ground))
        {
            isOverGround = false;
        }
    }*/
}