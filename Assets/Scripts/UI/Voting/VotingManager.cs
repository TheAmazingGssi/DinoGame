using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private MultiplayerButton[] buttons;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    [Header("Settings")]
    [SerializeField] private float voteDuration = 5f;

    public static event Action<int> OnVoteComplete;

    private Vote currentVote;

    private float timer;
    private int[] choices = new int[2];
    private int voted;

    private bool isVoting = false;

    private void Start()
    {
        votingPanel.SetActive(false);

        Vote vote = new Vote("description", new string[2] { "Raid their base", "Team up with rival herd" });
        currentVote = vote;
    }

    public void StartVote(Vote vote)
    {
        Debug.Log("starting vote");
        currentVote = vote;

        choices = new int[vote.Choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i] = 0;
        }

        SetupChoiceButtons(vote.Choices);

        descriptionText.text = vote.VoteDescription;
        voted = 0;

        timer = voteDuration;
        UpdateTimerDisplay();

        votingPanel.SetActive(true);
        isVoting = true;
    }

    private void Update()
    {
        if (PlayerEntity.PlayerList.Count > 1 && !isVoting) StartVote(currentVote);


        timer -= Time.deltaTime;
        UpdateTimerDisplay();

        if (isVoting && (timer <= 0 || voted >= PlayerEntity.PlayerList.Count))
        {
            Debug.Log(voted);
            CompleteVote();
        }
    }

    private void SetupChoiceButtons(string[] choices)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].gameObject.SetActive(false);
                buttons[i].button.interactable = false;
            }
            if (choiceTexts[i] != null)
            {
                choiceTexts[i].text = "";
            }
        }

        for (int i = 0; i < choices.Length && i < buttons.Length; i++)
        {
            if (buttons[i] != null && choiceTexts[i] != null)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].button.interactable = true;
                choiceTexts[i].text = choices[i];

                int choiceIndex = i;
                buttons[i].button.onClick.RemoveAllListeners();
                buttons[i].button.onClick.AddListener(() => CastVote(choiceIndex));
            }
        }

    }

    public void CastVote(int choiceIndex)
    {
        if (!isVoting)
            return;
        choices[choiceIndex]++;
        voted++;
        Debug.Log("player voted: " + voted);
        Debug.Log(choiceIndex + "vote casted");
    }

    private void CompleteVote()
    {
        isVoting = false;

        int maxVotes = choices.Max();
        List<int> topChoices = new List<int>();

        for(int i = 0; i < choices.Length; i++)
        {
            if (choices[i] > maxVotes) topChoices.Add(i);
        }

        int winningChoice = topChoices.Count == 1 ? topChoices[0] : topChoices[UnityEngine.Random.Range(0, topChoices.Count)];

        votingPanel.SetActive(false);

        OnVoteComplete?.Invoke(winningChoice);

        Debug.Log($"Vote completed. Winning choice: {(winningChoice == 0 ? currentVote.Choices[0] : currentVote.Choices[1])}");
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timer);
            timerText.text = $"{seconds}";
        }
    }
}