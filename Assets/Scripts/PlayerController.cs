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
        //very temporary just to check different controllers do different things
        rb.linearVelocity = movementInput * speed;
    }


    //Player Input component has events on the inspector, each one of these is wired to an event
    public void OnMove(InputAction.CallbackContext inputContext)
    {
        movementInput = inputContext.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext inputContext)
    {
        Debug.Log("Jumped");
    }
    public void OnAttack(InputAction.CallbackContext inputContext)
    {
        Debug.Log("Attack");
    }
    public void OnSpecial(InputAction.CallbackContext inputContext)
    {
        Debug.Log("Special");
    }
    public void OnBlock(InputAction.CallbackContext inputContext)
    {
        Debug.Log("Block");
    }

}
