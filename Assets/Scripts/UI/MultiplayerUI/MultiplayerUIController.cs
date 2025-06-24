using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MultiplayerUIController : MonoBehaviour
{
    [SerializeField] UISettings settings;

    bool ready;

    private CharacterType characterType;
    public MultiplayerButton CurrentlySelected;

    public void SetUp(CharacterType charType, MultiplayerButton defaultSelection)
    {
        characterType = charType;
        CurrentlySelected = defaultSelection;
        SetSelection(true);
    }
    private void SetSelection(bool value)
    {
        CurrentlySelected.characterIndicators[characterType].enabled = value;
    }

    private void MoveRight() => TryMove(CurrentlySelected.button.FindSelectableOnRight());
    private void MoveLeft() => TryMove(CurrentlySelected.button.FindSelectableOnLeft());
    private void MoveUp() => TryMove(CurrentlySelected.button.FindSelectableOnUp());
    private void MoveDown() => TryMove(CurrentlySelected.button.FindSelectableOnDown());
    private void TryMove(Selectable selectable)
    {
        if (selectable && selectable is Button)
        {
            //TODO: Remove the get component by making a script which keeps a refrence to all of the valid buttons
            SetSelection(false);
            MultiplayerButton multButton;
            if(selectable.TryGetComponent<MultiplayerButton>(out multButton))
                CurrentlySelected = multButton;
            SetSelection(true);
        }
    }
    public void OnNavigate(InputAction.CallbackContext inputContext)
    {
        //read the input
        Vector2 input = inputContext.ReadValue<Vector2>();

        //make sure we can even read input now
        if (ready)
            return;

        input = settings.CheckSettings(input);

        //now we can do stuff
        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            if (input.y > 0)
                MoveUp();
            else
                MoveDown();
        }
        else
        {
            if (input.x > 0)
                MoveRight();
            else
                MoveLeft();
        }
    }
    public void OnConrfimPressed(InputAction.CallbackContext inputContext)
    {
        if (ready) 
            return;

        ready = true;
        CurrentlySelected.characterIndicators[characterType].color = Color.white;
        CurrentlySelected.button.onClick.Invoke();
    }
}
