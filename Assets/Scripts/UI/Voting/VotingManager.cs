using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class VotingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private MultiplayerButton choice1Button;
    [SerializeField] private MultiplayerButton choice2Button;
    [SerializeField] private MultiplayerButton choice3Button;
    [SerializeField] private MultiplayerButton choice4Button;
    [SerializeField] private TextMeshProUGUI choice1Text;
    [SerializeField] private TextMeshProUGUI choice2Text;

    [Header("Settings")]
    [SerializeField] private float voteDuration = 5f;

    public static event Action<int> OnVoteComplete;

    private Vote currentVote;

    private float timer;
    private int[] votes = new int[2];
    private int voted = 0;

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

        votes[0] = 0;
        votes[1] = 0;

        descriptionText.text = vote.VoteDescription;
        choice1Text.text = vote.Choices[0];
        choice2Text.text = vote.Choices[1];

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

    public void CastVote(int choiceIndex)
    {
        if (!isVoting)
            return;
        votes[choiceIndex]++;
        voted++;
        Debug.Log("player voted: " + voted);
        Debug.Log(choiceIndex + "vote casted");
    }

    private void CompleteVote()
    {
        isVoting = false;

        int maxVotes = votes.Max();
        List<int> topChoices = new List<int>();

        for(int i = 0; i < votes.Length; i++)
        {
            if (votes[i] > maxVotes) topChoices.Add(i);
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