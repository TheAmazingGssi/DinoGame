using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerButton : MonoBehaviour
{
    
    [SerializeField] Image terryIndicator;
    [SerializeField] Image spencerIndicator;
    [SerializeField] Image parisIndicator;
    [SerializeField] Image andrewIndicator;

    public Button button;
    public Dictionary<CharacterType, Image> characterIndicators = new Dictionary<CharacterType, Image>();

    private int choiceIndex = -1;
    private bool isVoteButton = false;
    private VotingManager votingManager;

    void Awake()
    {
        characterIndicators.Add(CharacterType.Triceratops, terryIndicator);
        characterIndicators.Add(CharacterType.Spinosaurus, spencerIndicator);
        characterIndicators.Add(CharacterType.Parasaurolophus, parisIndicator);
        characterIndicators.Add(CharacterType.Therizinosaurus, andrewIndicator);

        foreach(Image img in characterIndicators.Values)
            img.enabled = false;
    }

    public void Initialize(int index, VotingManager manager)
    {
        Debug.Log($"Initializing index: {index}");
        choiceIndex = index;
        votingManager = manager;
        isVoteButton = true;
    }
    public void Select(PlayerEntity player)
    {
        Debug.Log($"INSIDE SELECT: isVoteButton: {isVoteButton}, voting manager: {votingManager.IsVoting}, choiseIndex: {choiceIndex}");
        if (isVoteButton && votingManager.IsVoting && choiceIndex != -1)
        {
            Debug.Log($"CASTING A VOTE FROM SELECT");
            votingManager.CastVote(player, choiceIndex);
        }
    }
}
