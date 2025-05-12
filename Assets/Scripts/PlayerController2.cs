using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSpeed = 720f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    private void Awake()
    {
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

    // Attack mechanic (to be implemented)
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Implement attack logic here
    }

    // Block mechanic (to be implemented)
    public void OnBlock(InputAction.CallbackContext context)
    {
        // Implement block logic here
    }

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