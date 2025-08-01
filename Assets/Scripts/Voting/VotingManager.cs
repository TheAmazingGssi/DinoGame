using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class VotingManager : MonoBehaviour
{
    [Header("Vote Panel References")]
    [SerializeField] private GameObject votingPanel;
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject ButtonsParent;
    [SerializeField] private GameObject background;
    [SerializeField] private ClockHandle hourHandle;
    [SerializeField] private ClockHandle minuteHandle;

    [Header("Lore Panel References")]
    [SerializeField] private GameObject lorePanel;

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
        Debug.Log("starting vote");

        background.SetActive(true);
        votingPanel.SetActive(true);
        currentVote = vote;

        choices = new int[vote.Choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i] = 0;
        }

        SetUpChoicesText(vote.Choices);
        SetupButtons(vote.Choices);

        descriptionText.text = vote.VoteDescription;
        TitleText.text = vote.VoteTitle;
        voted = 0;

        //StartVotingInteraction();

        timer = readDuration;
        isReading = true;
        UpdateTimerDisplay();
    }
    private void StartVotingInteraction()
    {
        ButtonsParent.SetActive(true);
        timer += voteDuration;
        isVoting = true;
        uiSpawner.SpawnControllers();
    }

    private void Update()
    {
        if(isVoting || isReading)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }

        
        if (isVoting && (timer <= 0 || voted >= PlayerEntity.PlayerList.Count))
        {
            Debug.Log("Timer " + timer + "amount " + voted + "entities " + PlayerEntity.PlayerList.Count);
           // Debug.Log(voted);
            CompleteVote();
        }
        
        if (isReading && (timer <= 0))
        {
            StartVotingInteraction();
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
        }
        ButtonsParent.SetActive(false);
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


        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.activeInHierarchy == true && i != winningChoice)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(WiningChoiceDisplay(winningChoice));



        Debug.Log($"Vote completed. Winning choice: {(winningChoice == 0 ? currentVote.Choices[0] : currentVote.Choices[1])}");
    }

    private IEnumerator WiningChoiceDisplay(int winningChoice)
    {
        yield return new WaitForSeconds(5);

        votingPanel.SetActive(false);

        OnVoteComplete?.Invoke(winningChoice);
        Debug.Log("Showing winning vote");
    }

    private void UpdateTimerDisplay()
    {
        //int seconds = Mathf.CeilToInt(timer);
        //timerText.text = $"{seconds}";
        hourHandle.SetHandle(voteDuration, timer);
        //minuteHandle.SetHandle(voteDuration, timer * amountOfMinuteRotations);
    }
}