using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VotingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private MultiplayerButton[] votingButtons = new MultiplayerButton[4];

    [Header("Settings")]
    [SerializeField] private float voteDuration = 10f;

    public static event Action<int> OnVoteComplete;

    private Vote currentVote;
    private float timer;
    private Dictionary<PlayerEntity, int> playerVotes = new Dictionary<PlayerEntity, int>();
    private HashSet<PlayerEntity> votedPlayers = new HashSet<PlayerEntity>();
    private bool isVoting = false;
    private List<MultiplayerUIController> activeUIControllers = new List<MultiplayerUIController>();

    private void Start()
    {
        votingPanel.SetActive(false);
        SetupVotingButtons();

        VoteChoice[] choices = new VoteChoice[]
        {
            new VoteChoice("Team up with Rival Herd", new TestVoteEffect("Gained alliance!")),
            new VoteChoice("Raid their Nest", new TestVoteEffect("Gained resources!"))
        };
        Vote testVote = new Vote("You arrive at the Western Desert. Will you team up with the bandits or raid their nest?", choices);
        StartVote(testVote);
    }

    private void SetupVotingButtons()
    {
        for (int i = 0; i < votingButtons.Length; i++)
        {
            int choiceIndex = i;
            votingButtons[i].button.onClick.AddListener(() => CastVote(choiceIndex));
        }
    }

    private void OnDestroy()
    {
        foreach (var button in votingButtons)
        {
            button.button.onClick.RemoveAllListeners();
        }
    }

    public void StartVote(Vote vote)
    {
        currentVote = vote;
        playerVotes.Clear();
        votedPlayers.Clear();
        activeUIControllers.Clear();

        descriptionText.text = vote.description;
        SetupChoiceButtons();

        SetupPlayerControllers();

        timer = voteDuration;
        UpdateTimerDisplay();
        votingPanel.SetActive(true);
        isVoting = true;

        Debug.Log($"Started vote: {vote.description}");
    }

    private void SetupChoiceButtons()
    {
        foreach (var button in votingButtons)
        {
            button.gameObject.SetActive(false);
            foreach (var indicator in button.characterIndicators.Values)
            {
                indicator.enabled = false;
            }
        }

        for (int i = 0; i < currentVote.choices.Length && i < votingButtons.Length; i++)
        {
            votingButtons[i].gameObject.SetActive(true);
            TextMeshProUGUI buttonText = votingButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = currentVote.choices[i].choiceText;
            }
        }
    }

    private void SetupPlayerControllers()
    {
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            MultiplayerUIController uiController = player.SpawnUIController(votingButtons[0]);
            activeUIControllers.Add(uiController);
        }
    }

    private void Update()
    {
        if (!isVoting)
            return;

        timer -= Time.deltaTime;
        UpdateTimerDisplay();

        if (timer <= 0 || votedPlayers.Count >= PlayerEntity.PlayerList.Count)
        {
            CompleteVote();
        }
    }

    private void CastVote(int choiceIndex)
    {
        if (!isVoting || choiceIndex >= currentVote.choices.Length)
            return;

        PlayerEntity votingPlayer = GetVotingPlayer(choiceIndex);
        if (votingPlayer == null || votedPlayers.Contains(votingPlayer))
            return;

        playerVotes[votingPlayer] = choiceIndex;
        votedPlayers.Add(votingPlayer);

        UpdatePlayerVoteStatus(votingPlayer, choiceIndex);

        Debug.Log($"Player {votingPlayer.CharacterType} voted for choice {choiceIndex}");
    }

    private PlayerEntity GetVotingPlayer(int choiceIndex)
    {
        foreach (var controller in activeUIControllers)
        {
            if (controller.CurrentlySelected == votingButtons[choiceIndex])
            {
                foreach (PlayerEntity player in PlayerEntity.PlayerList)
                {
                    if (player.CharacterType == GetControllerCharacterType(controller))
                    {
                        return player;
                    }
                }
            }
        }
        return null;
    }

    private CharacterType GetControllerCharacterType(MultiplayerUIController controller)
    {
        foreach (var kvp in controller.CurrentlySelected.characterIndicators)
        {
            if (kvp.Value.enabled)
            {
                return kvp.Key;
            }
        }
        return CharacterType.Triceratops;
    }

    private void UpdatePlayerVoteStatus(PlayerEntity player, int choiceIndex)
    {
        Image indicator = votingButtons[choiceIndex].characterIndicators[player.CharacterType];
        indicator.color = Color.white;
    }

    private void CompleteVote()
    {
        isVoting = false;

        int[] voteCounts = new int[currentVote.choices.Length];
        foreach (var vote in playerVotes.Values)
        {
            voteCounts[vote]++;
        }

        int winningChoice = 0;
        int maxVotes = voteCounts[0];
        for (int i = 1; i < voteCounts.Length; i++)
        {
            if (voteCounts[i] > maxVotes)
            {
                maxVotes = voteCounts[i];
                winningChoice = i;
            }
        }

        if (maxVotes == 0 || (maxVotes > 0 && HasTie(voteCounts, maxVotes)))
        {
            winningChoice = UnityEngine.Random.Range(0, currentVote.choices.Length);
        }

        currentVote.choices[winningChoice].effect.ApplyEffect(new List<PlayerEntity>(PlayerEntity.PlayerList));

        OnVoteComplete?.Invoke(winningChoice);
        Debug.Log($"Vote completed. Winning choice: {currentVote.choices[winningChoice].choiceText}");
    }

    private bool HasTie(int[] voteCounts, int maxVotes)
    {
        int maxVoteCount = 0;
        foreach (int count in voteCounts)
        {
            if (count == maxVotes)
                maxVoteCount++;
        }
        return maxVoteCount > 1;
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timer);
            timerText.text = $"{seconds}s";
        }
    }
}

[System.Serializable]
public class Vote
{
    public string description { get; private set; }
    public VoteChoice[] choices { get; private set; }

    public Vote(string description, VoteChoice[] choices)
    {
        this.description = description;
        this.choices = choices;
    }
}

[System.Serializable]
public class VoteChoice
{
    public string choiceText { get; private set; }
    public VoteEffect effect { get; private set; }

    public VoteChoice(string choiceText, VoteEffect effect)
    {
        this.choiceText = choiceText;
        this.effect = effect;
    }
}

public abstract class VoteEffect
{
    public abstract void ApplyEffect(List<PlayerEntity> allPlayers);
    public virtual bool ShouldTrigger(GameEvent eventType) { return false; }
}

public enum GameEvent
{
    NextLevel,
    FinalLevel,
    WaveStart,
    LevelComplete
}

public class TestVoteEffect : VoteEffect
{
    private string message;

    public TestVoteEffect(string message)
    {
        this.message = message;
    }

    public override void ApplyEffect(List<PlayerEntity> allPlayers)
    {
        Debug.Log($"Vote effect applied to {allPlayers.Count} players: {message}");
    }
}