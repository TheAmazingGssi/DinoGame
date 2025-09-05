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

    public int ChoiceIndex { get; private set; } = -1;
    public VotingManager VotingManager { get; private set; }

    private bool isVoteButton = true;
    public bool IsVoteButton => isVoteButton;

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
        ChoiceIndex = index;
        VotingManager = manager;
        isVoteButton = index != -1;
    }
    public void Select(PlayerEntity player)
    {
        if (isVoteButton && VotingManager != null && VotingManager.IsVoting && ChoiceIndex != -1)
            VotingManager.CastVote(player, ChoiceIndex);
    }
}
