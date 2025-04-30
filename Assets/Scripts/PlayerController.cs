using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 movementInput = Vector2.zero;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = movementInput * speed;
    }

    public void OnMove(InputAction.CallbackContext inputContext)
    {
        movementInput = inputContext.ReadValue<Vector2>();
    }
}
