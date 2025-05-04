
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    public Color Color = Color.white;


    public void OnNavigate(InputAction.CallbackContext inputContext)
    {
        Vector2 input = inputContext.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return;
        
        if (input.y < 0)
            Debug.Log("up");
        else
            Debug.Log("down");

    }
}
