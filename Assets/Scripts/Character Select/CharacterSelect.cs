
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float deadzone;
    [SerializeField] float inputCooldown;
    float counter = 0;
    public Color Color = Color.white;
    public CharSelectMultiplayerManager Manager;
    public UnityEvent UpdateColors;

    public void Update()
    {
        counter -= Time.deltaTime;
    }

    public void OnNavigate(InputAction.CallbackContext inputContext)
    {
        //read the input
        Vector2 input = inputContext.ReadValue<Vector2>();

        //make sure it isnt a wrong input
        if (input.magnitude < deadzone)
            return;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return;

        //not to swap through all the option in a single frame
        if (counter > 0)
            return;
        counter = inputCooldown;

        //now we can do stuff
        if (input.y < 0)
            ChangeColor(Manager.GetNextColor(Color));
        else
            ChangeColor(Manager.GetPreviousColor(Color));
    }
    private void ChangeColor(Color color)
    {
        Color = color;
        UpdateColors.Invoke();
    }
}
