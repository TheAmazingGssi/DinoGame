using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MultiplayerUIController : MonoBehaviour
{
    [SerializeField] UISettings settings;

    private PlayerEntity player;
    private CharacterType characterType;
    public MultiplayerButton CurrentlySelected;

    private bool hasReadied;
    private bool hasVoted;

    public void SetUp(CharacterType charType, MultiplayerButton defaultSelection, PlayerEntity playerEntity)
    {
        player = playerEntity;
        characterType = charType;
        CurrentlySelected = defaultSelection;

        // Reset semantic flags on each new setup
        hasReadied = false;
        hasVoted = false;

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
        Vector2 input = inputContext.ReadValue<Vector2>();

        if (hasVoted) return;

        input = settings.CheckSettings(input);
        if (input == Vector2.zero) return;

        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            if (input.y > 0) MoveUp();
            else MoveDown();
        }
        else
        {
            if (input.x > 0) MoveRight();
            else MoveLeft();
        }
    }
    public void OnConfirmPressed(InputAction.CallbackContext inputContext)
    {
        if (!inputContext.performed) return;

        if (CurrentlySelected == null) return;

        CurrentlySelected.characterIndicators[characterType].color = Color.white;

        var manager = CurrentlySelected.VotingManager;
        bool isVotingPhase = (manager != null) ? manager.IsVoting : CurrentlySelected.IsVoteButton;

        if (isVotingPhase)
        {
            if (CurrentlySelected.IsVoteButton && !hasVoted)
            {
                CurrentlySelected.Select(player);
                hasVoted = true;
            }
        }
        else
        {
            if (!hasReadied)
            {
                hasReadied = true;
                CurrentlySelected.button.onClick.Invoke();

                StartCoroutine(TemporarilyBlockConfirmOneFrame());
            }
        }
    }

    private IEnumerator TemporarilyBlockConfirmOneFrame()
    {
        yield return null;
    }



}
