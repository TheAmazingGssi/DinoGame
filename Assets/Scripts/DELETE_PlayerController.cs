using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;


public class PlayerController : MonoBehaviour
{
    // Constants
    private const int JUMP_FORCE = 5;

    [SerializeField] private PlayerTransformData playerTransform;
    
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


    private void Awake()
    {
        playerTransform.PlayerTransform = transform;
    }
    
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
