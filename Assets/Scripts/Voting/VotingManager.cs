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
    [SerializeField] private ClockHandle hourHandle;
    [SerializeField] private ClockHandle minuteHandle;

    [Header("Lore Panel References")]
    [SerializeField] private TextMeshProUGUI lorePanelTitleText;
    [SerializeField] private GameObject lorePanel;
    [SerializeField] private TextMeshProUGUI lorePanelText;

    [Header("Winning Vote Panel References")]
    [SerializeField] private GameObject winningVotePanel;
    [SerializeField] private TextMeshProUGUI winningVoteText;

    [SerializeField] private MultiplayerButton[] buttons;
    [SerializeField] private TextMeshProUGUI[] choicesTexts;

    [SerializeField] private GameObject testButton;

    [SerializeField] private UIControllerSpawner uiSpawner;

    [Header("Settings")]
    [SerializeField] private float readDuration = 20f;
    [SerializeField] private float voteDuration = 20f;
    [SerializeField] private int amountOfMinuteRotations = 3;

    public static event Action<int> OnVoteComplete;

    private Vote currentVote;
    private float timer;
    private int[] choices;
    private int voted;

    private bool isVoting = false;
    private bool isReading = false;

    public void StartVote(Vote vote)
    {
        Debug.Log("Starting vote...");

        currentVote = vote;

        background.SetActive(true);
        lorePanel.SetActive(true);
        votingPanel.SetActive(false);
        winningVotePanel.SetActive(false);

        descriptionText.text = vote.VoteDescription;
        titleText.text = vote.VoteTitle;

        lorePanelText.text = vote.VoteDescription;
        lorePanelTitleText.text = vote.VoteTitle;

        choices = new int[vote.Choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i] = 0;
        }

        SetUpChoicesText(vote.Choices);
        SetupButtons(vote.Choices);

        voted = 0;

        timer = readDuration;
        isReading = true;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isReading || isVoting)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }

        if (isReading && timer <= 0)
        {
            isReading = false;
            lorePanel.SetActive(false);
            votingPanel.SetActive(true);
            StartVotingInteraction();
        }

        if (isVoting && (timer <= 0 || voted >= PlayerEntity.PlayerList.Count))
        {
            Debug.Log("Vote ended. Timer: " + timer + ", Votes: " + voted + "/" + PlayerEntity.PlayerList.Count);
            CompleteVote();
        }
    }

    private void StartVotingInteraction()
    {
        buttonsParent.SetActive(true);
        timer = voteDuration;
        isVoting = true;
        uiSpawner.SpawnControllers();
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
        }

        buttonsParent.SetActive(false);
    }

    public void CastVote(int choiceIndex)
    {
        if (!isVoting) return;

        choices[choiceIndex]++;
        voted++;
        Debug.Log($"Player voted ({voted} total). Choice: {choiceIndex}");
    }

    private void CompleteVote()
    {
        isVoting = false;

        int maxVotes = choices.Max();
        List<int> topChoices = new List<int>();

        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i] >= maxVotes)
                topChoices.Add(i);
        }

        int winningChoice = topChoices.Count == 1 ? topChoices[0] : topChoices[UnityEngine.Random.Range(0, topChoices.Count)];

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

    private void UpdateTimerDisplay()
    {
        hourHandle.SetHandle(voteDuration, timer);
        // minuteHandle.SetHandle(voteDuration, timer * amountOfMinuteRotations); // Optional
    }
}
