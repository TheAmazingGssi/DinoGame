using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VotingManager : MonoBehaviour
{
    [Header("Vote Panel References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject background;

    [Header("Lore Panel References")]
    [SerializeField] private TextMeshProUGUI lorePanelTitleText;
    [SerializeField] private GameObject lorePanel;
    [SerializeField] private TextMeshProUGUI lorePanelText;
    [SerializeField] private MultiplayerButton continueButton;

    [Header("Winning Vote Panel References")]
    [SerializeField] private GameObject winningVotePanel;
    [SerializeField] private TextMeshProUGUI winningVoteText;

    [SerializeField] private MultiplayerButton[] buttons;
    [SerializeField] private TextMeshProUGUI[] choicesTexts;

    [SerializeField] private UIControllerSpawner uiSpawner;

    public static event Action<int> OnVoteComplete;

    private Dictionary<PlayerEntity, int> playerVotes = new Dictionary<PlayerEntity, int>();

    private Vote currentVote;
    private int[] choices;
    private int voted;
    private int readyPlayers;

    private bool isVoting = false;
    public bool IsVoting => isVoting;

    public void StartVote(Vote vote)
    {
        currentVote = vote;

        isVoting = false;
        voted = 0;
        readyPlayers = 0;

        background.SetActive(true);
        lorePanel.SetActive(true);
        votingPanel.SetActive(false);
        buttonsParent.SetActive(false);
        winningVotePanel.SetActive(false);

        descriptionText.text = currentVote.VoteDescription;
        titleText.text = currentVote.VoteTitle;
        lorePanelText.text = currentVote.VoteDescription;
        lorePanelTitleText.text = currentVote.VoteTitle;

        choices = new int[currentVote.Choices.Length];
        for (int i = 0; i < choices.Length; i++) choices[i] = 0;

        continueButton.gameObject.SetActive(true);
        continueButton.Initialize(-1, this);
        uiSpawner.SpawnControllers(continueButton);
    }

    private void VotingPhase()
    {
        lorePanel.SetActive(false);
        votingPanel.SetActive(true);

        voted = 0;
        playerVotes.Clear();

        SetUpChoicesText(currentVote.Choices);
        SetupButtons(currentVote.Choices);
        buttonsParent.SetActive(true);

        uiSpawner.SpawnControllers(buttons[0]);

        isVoting = true;
    }


    public void MoveToVotes()
    {
        readyPlayers++;
        if (readyPlayers >= PlayerEntity.PlayerList.Count) VotingPhase();
    }
    
    private void SetUpChoicesText(string[] text)
    {
        for (int i = 0; i < choicesTexts.Length; i++)
        {
            if (i < text.Length)
                choicesTexts[i].text = text[i];
            else
                choicesTexts[i].text = "";
        }
    }

    private void SetupButtons(string[] text)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            bool hasText = i < text.Length;
            buttons[i].gameObject.SetActive(hasText);
            buttons[i].button.interactable = hasText;

            if (hasText)
                buttons[i].Initialize(i, this);
        }
    }

    public void CastVote(PlayerEntity player, int choiceIndex)
    {
        if (!isVoting) return;

        if (playerVotes.ContainsKey(player))
        {
            choices[playerVotes[player]]--;
            playerVotes[player] = choiceIndex;
        }
        else
        {
            playerVotes[player] = choiceIndex;
            voted++;
        }

        SelectionSoundPlayer.instance.PlaySelectionSound();
        choices[choiceIndex]++;
        
        if (voted >= PlayerEntity.PlayerList.Count)
        {
            Debug.Log($"Vote completed, number voted: {voted}");
            CompleteVote();
        }
    }

    private void CompleteVote()
    {
        isVoting = false;

        int maxVotes = choices.Max();
        List<int> topChoices = new List<int>();

        for (int i = 0; i < choices.Length; i++)
            if (choices[i] == maxVotes)
                topChoices.Add(i);

        int winningChoice;

        if (topChoices.Count == 1)
        {
            winningChoice = topChoices[0];
        }
        else
        {
            PlayerEntity highScoringPlayer = GameManager.Instance.GetHighestScorePlayer();
            playerVotes.TryGetValue(highScoringPlayer, out int votedChoice);
            winningChoice = votedChoice;
        }
        
        votingPanel.SetActive(false);
        StartCoroutine(WiningChoiceDisplay(winningChoice));

        Debug.Log($"Vote completed. Winning choice: {currentVote.Choices[winningChoice]}");
    }

    private IEnumerator WiningChoiceDisplay(int winningChoice)
    {
        winningVotePanel.SetActive(true);
        winningVoteText.text = currentVote.Choices[winningChoice];

        yield return new WaitForSeconds(5f);

        winningVotePanel.SetActive(false);
        background.SetActive(false);

        OnVoteComplete?.Invoke(winningChoice);
    }
}
