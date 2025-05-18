using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterSelect : MonoBehaviour
{
    public PlayerInput PlayerInput;
    [SerializeField] float deadzone;
    [SerializeField] float inputCooldown;
    float counter = 0;
    public Color Color = Color.white;
    public CharSelectMultiplayerManager Manager;
    public UnityEvent UpdateColors;
    public UnityEvent UpdateReady;
    public bool ready = false;
    public UnityEvent FinalizeSelection;
    

    public void Update()
    {
        counter -= Time.deltaTime;
    }

    public void OnNavigate(InputAction.CallbackContext inputContext)
    {
        //read the input
        Vector2 input = inputContext.ReadValue<Vector2>();

        //make sure it isnt a wrong input
        if (input.magnitude < deadzone) //no drift joystick
            return;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) //make sure its up and down not left and right
            return;
        if (ready) //make sure we can even read up down input now
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
    public void OnXPressed()
    {
        ready = !ready;
        UpdateReady.Invoke();
    }
    private void ChangeColor(Color color)
    {
        Color = color;
        UpdateColors.Invoke();
    }
}
