using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private const string Ground = "Ground";

    [SerializeField] EnemyManager manager;

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
            moveDirection = Vector3.zero;
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