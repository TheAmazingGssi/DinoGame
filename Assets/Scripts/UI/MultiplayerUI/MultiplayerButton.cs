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

    private int choiceIndex;
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
        choiceIndex = index;
        votingManager = manager;
    }
    public void Select(PlayerEntity player)
    {
        if (votingManager.IsVoting)
            votingManager.CastVote(player, choiceIndex);

/*        if (characterIndicators.TryGetValue(player.CharacterType, out var indicator))
            indicator.enabled = true;*/
    }
}
