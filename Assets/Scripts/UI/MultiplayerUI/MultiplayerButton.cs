using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerButton : MonoBehaviour
{
    
    [SerializeField] Image terryIndicator;
    [SerializeField] Image spencerIndicator;
    [SerializeField] Image parisIndicator;
    [SerializeField] Image andrewIndicator;

    [SerializeField] Image terryCrown;
    [SerializeField] Image spencerCrown;
    [SerializeField] Image parisCrown;
    [SerializeField] Image andrewCrown;

    public Button button;
    public Dictionary<CharacterType, Image> characterIndicators = new Dictionary<CharacterType, Image>();
    public Dictionary<CharacterType, Image> crowns = new Dictionary<CharacterType, Image>();

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

        crowns.Add(CharacterType.Triceratops, terryCrown);
        crowns.Add(CharacterType.Spinosaurus, spencerCrown);
        crowns.Add(CharacterType.Parasaurolophus, parisCrown);
        crowns.Add(CharacterType.Therizinosaurus, andrewCrown);

        foreach (Image img in characterIndicators.Values)
            img.enabled = false;
        foreach (Image img in crowns.Values)
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
