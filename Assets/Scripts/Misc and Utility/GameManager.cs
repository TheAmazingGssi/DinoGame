using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] VotingManager stagesVote;
    [SerializeField] Vote vote;
    public int LevelNumber = 1;
    public int FinaleLevel = 3;
    public Dictionary<Vote, int> FinaleLevelEffects = new Dictionary<Vote, int>();
    public static GameManager Instance;
    private int enemiesOnStage = 0;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (LevelNumber == FinaleLevel)
        {
            foreach (var vote in FinaleLevelEffects)
            {
                Vote currentVote = vote.Key;
                int choiceIndex = vote.Value;
                currentVote.ApplyEffects(choiceIndex);
            }
            FinaleLevelEffects.Clear();
        }
    }

    private void OnEnable() => VotingManager.OnVoteComplete += HandleVoteComplete;
    private void OnDisable() => VotingManager.OnVoteComplete -= HandleVoteComplete;

    void Update()
    {
    }

    public void IncrementDeathCount()
    {
        enemiesOnStage--;
        if (enemiesOnStage <= 0)
        {
            StartVote();
        }
    }

    public void SetWaveSize(int amount)
    {
        enemiesOnStage += amount;
    }

    public void StartVote()
    {
        stagesVote.StartVote(vote);
    }

    private void HandleVoteComplete(int winningChoice)
    {
        vote.ApplyEffects(winningChoice);
    }

    public PlayerEntity GetHighestScorePlayer()
    {
        PlayerEntity highestScorePlayer = null;
        int highestScore = int.MinValue;
        foreach (var player in PlayerEntity.PlayerList)
        {
            int score = player.MainPlayerController.GetScore();
            if (score > highestScore)
            {
                highestScore = score;
                highestScorePlayer = player;
            }
        }
        return highestScorePlayer;
    }

    public PlayerEntity GetLowestScorePlayer()
    {
        PlayerEntity lowestScorePlayer = null;
        int lowestScore = int.MaxValue;
        foreach (var player in PlayerEntity.PlayerList)
        {
            int score = player.MainPlayerController.GetScore();
            if (score < lowestScore)
            {
                lowestScore = score;
                lowestScorePlayer = player;
            }
        }
        return lowestScorePlayer;
    }
}