using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class VotingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private MultiplayerButton[] buttons;
    [SerializeField] private TextMeshProUGUI[] buttonsTexts;
    [SerializeField] private TextMeshProUGUI[] choicesTexts;

    [Header("Settings")]
    [SerializeField] private float voteDuration = 20f;

    public static event Action<int> OnVoteComplete;

    private Vote currentVote;

    private float timer;
    private int[] choices = new int[2];
    private int voted;

    private bool isVoting = false;

    private void Start()
    {
        votingPanel.SetActive(false);

        Vote vote = new Vote(
            "Gain local alliances for future help\r\nVS.\r\ngaining supply that will help now",
            new string[3] { "Raid their base\r\ngain max health boost.\n<color=red>BUT</color>\nOn the next level, face more enemies\r\n",
                "Team up with Rival Herd:\r\nbecome stronger in the final level\n<color=red>BUT</color>\n[Terry] takes more damage in the final level",
                "Test"},
            new string[3] { "Raid their base",
                "Team up with rival herd", "third option" });
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

        SetUpChoicesText(vote.Choices);
        SetupButtons(vote.ButtonTexts);

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

        if(isVoting)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }


        if (isVoting && (timer <= 0 || voted >= PlayerEntity.PlayerList.Count))
        {
            Debug.Log(voted);
            CompleteVote();
        }
    }

    private void SetUpChoicesText(string[] text)
    {
        for (int i = 0; i < choicesTexts.Length; i++)
        {
            if (choicesTexts[i] != null) choicesTexts[i].text = "";
        }
        for (int i = 0; i < text.Length; i++)
        {
            if (choicesTexts[i] != null) choicesTexts[i].text = text[i];
        }
    }
    private void SetupButtons(string[] text)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].gameObject.SetActive(false);
                buttons[i].button.interactable = false;
            }
            if (buttonsTexts[i] != null) ; buttonsTexts[i].text = "";
        }

        for (int i = 0; i < text.Length && i < buttons.Length; i++)
        {
            if (buttons[i] != null && buttonsTexts[i] != null)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].button.interactable = true;
                buttonsTexts[i].text = text[i];
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