using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] UISettings settings;
    [SerializeField] SoundPlayer soundPlayer;
    
    public CharacterType SelectedCharacter = CharacterType.Triceratops;
    public CharSelectMultiplayerManager Manager;
    public UnityEvent UpdateColors;
    public UnityEvent UpdateReady;
    public bool ready = false;
    public UnityEvent FinalizeSelection;
    

    public void OnNavigate(InputAction.CallbackContext inputContext)
    {
        //read the input
        Vector2 input = inputContext.ReadValue<Vector2>();
        //Vector2 rawInput = input;
        input = settings.CheckSettings(input);
        //Debug.Log($"Raw Input: {rawInput} sanitized: {input}");

        if (ready) //make sure we can even read up down input now
            return;
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y)) //make sure its up and down not left and right. Also deals with cases where the input is (0, 0)
            return;

        //now we can do stuff

        soundPlayer.PlaySound(0);
        if (input.y < 0)
            ChangeColor(Manager.GetNextCharacter(SelectedCharacter));
        else
            ChangeColor(Manager.GetPreviousCharacter(SelectedCharacter));
    }
    public void OnXPressed(InputAction.CallbackContext inputContext)
    {
        soundPlayer.PlaySound(1);
        ready = true;
        UpdateReady.Invoke();
    }
    public void OnCancel(InputAction.CallbackContext inputContext)
    {
        soundPlayer.PlaySound(1);
        ready = false;
        UpdateReady.Invoke();
    }

    private void ChangeColor(CharacterType characterType)
    {
        SelectedCharacter = characterType;
        UpdateColors.Invoke();
    }
}
