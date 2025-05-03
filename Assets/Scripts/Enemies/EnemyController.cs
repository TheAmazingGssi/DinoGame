using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.0f;
    [Tooltip("Range the enemy can sense the player")]
    [SerializeField] private float detectionRange = 7.0f;

    [Header("Referenses")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private Transform player;

    private Vector2 moveDirection = Vector2.zero;
    private bool facingRight = true;
    private bool isOverGround = true;

    private void Awake()
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
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && isOverGround)
        {
            moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        }
        else
        {
            moveDirection = Vector2.zero;
        }

        if (moveDirection.x > 0 && !facingRight)
        {
            FlipSprite();
        }
        else if (moveDirection.x < 0 && facingRight)
        {
            FlipSprite();
        }
    }


    private void StayInBounds() //stay in walkable area
    {
        if (isOverGround)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void FlipSprite()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
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