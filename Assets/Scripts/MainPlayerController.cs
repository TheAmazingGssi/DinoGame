using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class MainPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private bool facingRight = true;
    
    [Header("Required Components")]
    [SerializeField] private PlayerTransformData playerTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Block Variables")]
    public float damageWindow = 0.5f; // Time
    
    [Header("Attack Variables")]
    [SerializeField] GameObject rightMeleeColliderGO;
    [SerializeField] GameObject leftMeleeColliderGO;
    [SerializeField] private float enableDuration = 0.2f; // Duration collider stays active
    [SerializeField] private float disableDelay = 0.5f; // Delay before disabling collider
    [SerializeField] private float attackCooldown = 1f; // Cooldown between attacks
    [SerializeField] private int attackDamage = 10; // Damage dealt to enemies
    
    private float lastAttackTime; // Track the last attack time for cooldown
    private bool canAttack = true; // Check if player can attack (cooldown)

    private bool isOverGround = true;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttacking = false;
    private bool isBlocking = false;
    
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private BoxCollider2D rightMeleeCollider;
    private BoxCollider2D leftMeleeCollider;

    public static bool CanBeDamaged = true;

    private void Awake()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true; // Prevent physics rotation
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // Called by Input System when movement input is detected
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        // Normalize input to prevent faster diagonal movement
        Vector2 targetVelocity = moveInput.normalized * moveSpeed;

        // Smoothly accelerate towards target velocity
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Apply velocity to Rigidbody2D
        rb.linearVelocity = currentVelocity;

        // Rotate to face movement direction
        /*
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 90f; // -90 for top-down sprite
            float currentAngle = transform.rotation.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.rotation = newAngle;
        }
        */
    }

    public void OnAttack(InputAction.CallbackContext inputContext)
    {
        if (canAttack)
        {
            canAttack = false; // Prevent attacking again until cooldown is over
            lastAttackTime = Time.time; // Record the attack time
            StartCoroutine(EnableMeleeCollider()); // Start the enable coroutine
        }
    }
    public void OnSpecial(InputAction.CallbackContext inputContext)
    {
        Debug.Log("Special");
    }
    
    public void OnBlock(InputAction.CallbackContext inputContext)
    {
        StartCoroutine(BlockDamageWindow());
    }
    
    IEnumerator BlockDamageWindow()
    {
        isBlocking = true;
        CanBeDamaged = true;
        
        yield return new WaitForSeconds(damageWindow);
        
        CanBeDamaged = false;
        isBlocking = false;
    }

    
    // Coroutine to enable the melee collider
    private IEnumerator EnableMeleeCollider()
    {
        if (leftMeleeCollider != null && rightMeleeCollider != null)
        {
            if (facingRight)
            {
                rightMeleeColliderGO.SetActive(true);  // Enable the collider
            }
            else
            {
                leftMeleeColliderGO.SetActive(true);  // Enable the collider
            }
            
            yield return new WaitForSeconds(enableDuration); // Wait for the specified duration
            StartCoroutine(DisableMeleeCollider()); // Start the disable coroutine
        }
    }

    // Coroutine to disable the melee collider
    private IEnumerator DisableMeleeCollider()
    {
        yield return new WaitForSeconds(disableDelay); // Wait for the specified delay
        
        if (leftMeleeCollider != null && rightMeleeCollider != null)
        {
            if (facingRight)
            {
                rightMeleeColliderGO.SetActive(false); 
            }
            else
            {
                leftMeleeColliderGO.SetActive(false);
            }
        }
    }

    /*
    private IEnumerator Land()
    {
        yield return new WaitForSeconds(jumpDuration); // Wait for the specified delay
        
        //if ()
    }
    */

    // Special attack mechanic (to be implemented)
    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        // Implement special attack logic here
    }

    // Emote mechanic (to be implemented)
    public void OnEmote(InputAction.CallbackContext context)
    {
        // Implement emote logic here
    }

    // Revive mechanic (to be implemented)
    public void OnRevive(InputAction.CallbackContext context)
    {
        // Implement revive logic here
    }
}