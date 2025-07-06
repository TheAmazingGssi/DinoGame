using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
public class VotingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private MultiplayerButton[] buttons;
    [SerializeField] private TextMeshProUGUI[] buttonsTexts;
    [SerializeField] private TextMeshProUGUI[] choicesTexts;

    [SerializeField] private UIControllerSpawner uiSpawner;


    [Header("Settings")]
    [SerializeField] private float voteDuration = 20f;

    public static event Action<int> OnVoteComplete;

    private Vote currentVote;

    private float timer;
    private int[] choices;
    private int voted;

    private bool isVoting = false;
    private bool isVotingDebug = false;

    private void Start()
    {
        /*
        Vote vote4 = new Vote(
            "Gain local alliances for future help\r\nVS.\r\ngaining supply that will help now",
            new string[4] { "Raid their base\r\ngain max health boost.\n<color=red>BUT</color>\nOn the next level, face more enemies\r\n",
                "Team up with Rival Herd:\r\nbecome stronger in the final level\n<color=red>BUT</color>\n[Terry] takes more damage in the final level",
                "Test", "test2"},
            new string[4] { "Raid their base",
                "Team up with rival herd", "third option", "fourth option" });
                Vote vote2 = new Vote(
            "Gain local alliances for future help\r\nVS.\r\ngaining supply that will help now",
            new string[2] { "Raid their base\r\ngain max health boost.\n<color=red>BUT</color>\nOn the next level, face more enemies\r\n",
                "Team up with Rival Herd:\r\nbecome stronger in the final level\n<color=red>BUT</color>\n[Terry] takes more damage in the final level"},
            new string[2] { "Raid their base",
                "Team up with rival herd"});
        currentVote = vote2;
        */
    }

    public void StartVote(Vote vote)
    {
     //   Debug.Log("starting vote");
        votingPanel.SetActive(true);
        currentVote = vote;

        choices = new int[vote.Choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i] = 0;
        }

        SetUpChoicesText(vote.Choices);
        SetupButtons(vote.ButtonTexts);

        descriptionText.text = vote.VoteDescription;
        TitleText.text = vote.VoteTitle;
        voted = 0;

        timer = voteDuration;
        UpdateTimerDisplay();

        
        uiSpawner.SpawnControllers();
        isVoting = true;
        isVotingDebug = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !isVoting && !isVotingDebug) StartVote(currentVote);

        if(isVoting)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }


        if (isVoting && (timer <= 0 || voted >= PlayerEntity.PlayerList.Count))
        {
           // Debug.Log(voted);
            CompleteVote();
        }
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
            buttonsTexts[i].text = hasText ? text[i] : "";
        }
    }

    public void CastVote(int choiceIndex)
    {
        if (!isVoting) return;
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
            if (choices[i] >= maxVotes) topChoices.Add(i);
        }

        int winningChoice = topChoices.Count == 1 ? topChoices[0] : topChoices[UnityEngine.Random.Range(0, topChoices.Count)]; //random until we add xp

        votingPanel.SetActive(false);

        OnVoteComplete?.Invoke(winningChoice);

        Debug.Log($"Vote completed. Winning choice: {(winningChoice == 0 ? currentVote.Choices[0] : currentVote.Choices[1])}");
    }

    private void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(timer);
        timerText.text = $"{seconds}";
    }
}