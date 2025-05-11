/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;


public class PlayerController : MonoBehaviour
{
    // Constants
    private const int JUMP_FORCE = 5;
    
    //Input Variables
    Vector2 movementInput = Vector2.zero;
    HID.Button jumpButton;
    HID.Button attackButton;
    HID.Button specialButton;
    HID.Button blockButton;
    
    //Movement variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D groundCheckCollider;
    private bool facingRight = true;
    
    //Jump variables
    [SerializeField] private float jumpDuration = 0.6f; // Jump air time legth
    
    //Attack variables
    [SerializeField] private GameObject leftMeleeCollider; // Melee attack collider
    [SerializeField] private GameObject rightMeleeCollider; // Melee attack collider
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
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Update attack availability based on cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            canAttack = true;
        }
        
        //very temporary just to check different controllers do different things
        rigidBody.linearVelocity = movementInput * moveSpeed;
    }


    //Player Input component has events on the inspector, each one of these is wired to an event
    public void OnMove(InputAction.CallbackContext inputContext)
    {
        movementInput = inputContext.ReadValue<Vector2>();
        animator.SetBool("isMoving", movementInput != Vector2.zero); // Set animator parameter based on movement input
    }
    
    public void OnJump(InputAction.CallbackContext inputContext)
    {
        if (isGrounded)
        {
            rigidBody.linearVelocityY += JUMP_FORCE; // Apply jump force to the rigidbody
            isJumping = true;
            animator.SetBool("isJumping", isJumping);
        }
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
        Debug.Log("Block");
    }

    
    // Coroutine to enable the melee collider
    private IEnumerator EnableMeleeCollider()
    {
        if (leftMeleeCollider != null && rightMeleeCollider != null)
        {
            if (facingRight)
            {
                rightMeleeCollider.SetActive(true);  // Enable the collider
            }
            else
            {
                leftMeleeCollider.SetActive(true);  // Enable the collider
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
                rightMeleeCollider.SetActive(false); 
            }
            else
            {
                leftMeleeCollider.SetActive(false);
            }
        }
    }

    private IEnumerator Land()
    {
        yield return new WaitForSeconds(jumpDuration); // Wait for the specified delay
        
        //if ()
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerController : MonoBehaviour
{
    // Constants
    private const int JUMP_FORCE = 5;
    
    // Input Variables
    Vector2 movementInput = Vector2.zero;
    HID.Button jumpButton;
    HID.Button attackButton;
    HID.Button specialButton;
    HID.Button blockButton;
    
    // Movement variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private LayerMask groundLayer; // Layer for ground detection
    private bool facingRight = true;
    
    // Jump variables
    [SerializeField] private float jumpDuration = 0.6f; // Jump air time length
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius for ground check
    
    // Attack variables
    [SerializeField] private GameObject leftMeleeCollider; // Melee attack collider
    [SerializeField] private GameObject rightMeleeCollider; // Melee attack collider
    [SerializeField] private float enableDuration = 0.2f; // Duration collider stays active
    [SerializeField] private float disableDelay = 0.5f; // Delay before disabling collider
    [SerializeField] private float attackCooldown = 1f; // Cooldown between attacks
    [SerializeField] private int attackDamage = 10; // Damage dealt to enemies
    
    private float lastAttackTime; // Track the last attack time for cooldown
    private bool canAttack = true; // Check if player can attack (cooldown)
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttacking = false;
    
    void Start()
    {
        // Ensure components are assigned
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (groundCheckCollider == null) Debug.LogError("GroundCheckCollider not assigned!");
    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckCollider.bounds.center, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        // Update attack availability based on cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            canAttack = true;
        }

        // Update animator with current velocity
        animator.SetFloat("speed", Mathf.Abs(movementInput.x));

        // Handle facing direction
        if (movementInput.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (movementInput.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        // Apply movement in FixedUpdate for physics consistency
        Vector2 velocity = rigidBody.linearVelocity;
        velocity.x = movementInput.x * moveSpeed;
        rigidBody.linearVelocity = velocity;
    }

    // Player Input component events
    public void OnMove(InputAction.CallbackContext inputContext)
    {
        movementInput = inputContext.ReadValue<Vector2>();
        animator.SetBool("isMoving", movementInput != Vector2.zero); // Set animator parameter
    }
    
    public void OnJump(InputAction.CallbackContext inputContext)
    {
        if (inputContext.performed && isGrounded)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, JUMP_FORCE);
            isJumping = true;
            isGrounded = false;
            animator.SetBool("isJumping", isJumping);
            StartCoroutine(Land());
        }
    }
    
    public void OnAttack(InputAction.CallbackContext inputContext)
    {
        if (inputContext.performed && canAttack)
        {
            canAttack = false;
            lastAttackTime = Time.time;
            isAttacking = true;
            animator.SetTrigger("attack");
            StartCoroutine(EnableMeleeCollider());
        }
    }
    
    public void OnSpecial(InputAction.CallbackContext inputContext)
    {
        if (inputContext.performed)
        {
            // Placeholder for special ability
            Debug.Log("Special ability activated!");
            animator.SetTrigger("special");
        }
    }
    
    public void OnBlock(InputAction.CallbackContext inputContext)
    {
        if (inputContext.performed)
        {
            // Placeholder for block ability
            Debug.Log("Blocking!");
            animator.SetBool("isBlocking", true);
        }
        else if (inputContext.canceled)
        {
            animator.SetBool("isBlocking", false);
        }
    }

    // Coroutine to enable the melee collider
    private IEnumerator EnableMeleeCollider()
    {
        if (leftMeleeCollider != null && rightMeleeCollider != null)
        {
            if (facingRight)
            {
                rightMeleeCollider.SetActive(true);
            }
            else
            {
                leftMeleeCollider.SetActive(true);
            }
            
            yield return new WaitForSeconds(enableDuration);
            isAttacking = false;
            StartCoroutine(DisableMeleeCollider());
        }
    }

    // Coroutine to disable the melee collider
    private IEnumerator DisableMeleeCollider()
    {
        yield return new WaitForSeconds(disableDelay);
        
        if (leftMeleeCollider != null && rightMeleeCollider != null)
        {
            if (facingRight)
            {
                rightMeleeCollider.SetActive(false);
            }
            else
            {
                leftMeleeCollider.SetActive(false);
            }
        }
    }

    // Coroutine to handle landing after jump
    private IEnumerator Land()
    {
        yield return new WaitForSeconds(jumpDuration);
        
        // Check if player is grounded after jump duration
        if (isGrounded)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
    }

    // Flip the character sprite based on movement direction
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}