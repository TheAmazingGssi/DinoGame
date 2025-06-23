//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class VotingSystem : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private GameObject votingPanel;
//    [SerializeField] private TextMeshProUGUI descriptionText;
//    [SerializeField] private TextMeshProUGUI timerText;
//    [SerializeField] private Button choice1Button;
//    [SerializeField] private Button choice2Button;
//    [SerializeField] private TextMeshProUGUI choice1Text;
//    [SerializeField] private TextMeshProUGUI choice2Text;

//    [Header("Settings")]
//    [SerializeField] private float voteDuration = 5f;

//    public static event Action<int> OnVoteComplete;

//    private Vote currentVote;
//    private float timer;
//    private int[] votes = new int[2];
//    private bool isVoting = false;

//    private void Start()
//    {
//        votingPanel.SetActive(false);

//        choice1Button.onClick.AddListener(() => CastVote(0));

//        choice2Button.onClick.AddListener(() => CastVote(1));

//        Vote vote = new Vote("description", "one", "two");
//        StartVote(vote);
//    }

//    private void OnDestroy()
//    {
//        choice1Button.onClick.RemoveAllListeners();


//        choice2Button.onClick.RemoveAllListeners();
//    }

//    public void StartVote(Vote vote)
//    {
//        currentVote = vote;

//        votes[0] = 0;
//        votes[1] = 0;

//        descriptionText.text = vote.voteDescription;
//        choice1Text.text = vote.choice1;
//        choice2Text.text = vote.choice2;

//        timer = voteDuration;
//        UpdateTimerDisplay();

//        votingPanel.SetActive(true);
//        isVoting = true;
//    }

//    private void Update()
//    {
//        if (!isVoting)
//            return;

//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            Debug.Log("1");
//            CastVote(0);
//        }
//        else if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            Debug.Log("2");

//            CastVote(1);
//        }

//        timer -= Time.deltaTime;
//        UpdateTimerDisplay();

//        if (timer <= 0)
//        {
//            CompleteVote();
//        }
//    }

//    private void CastVote(int choiceIndex)
//    {
//        if (!isVoting)
//            return;
//        votes[choiceIndex]++;
//    }

//    private void CompleteVote()
//    {
//        isVoting = false;

//        int winningChoice;

//        if (votes[0] > votes[1])
//        {
//            winningChoice = 0;
//        }
//        else if (votes[1] > votes[0])
//        {
//            winningChoice = 1;
//        }
//        else
//        {
//            winningChoice = UnityEngine.Random.Range(0, 2);
//        }

//        votingPanel.SetActive(false);

//        OnVoteComplete?.Invoke(winningChoice);

//        Debug.Log($"Vote completed. Winning choice: {(winningChoice == 0 ? currentVote.choice1 : currentVote.choice2)}");
//    }

//    private void UpdateTimerDisplay()
//    {
//        if (timerText != null)
//        {
//            int seconds = Mathf.CeilToInt(timer);
//            timerText.text = $"{seconds}";
//        }
//    }
//}