using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private VotingManager stagesVote;
    [SerializeField] private Vote vote;
    [field: SerializeField] public SpawnerManager SpawnerManager {get; private set;}
    public int LevelNumber = 1;
    public int FinaleLevel = 3;
    public Dictionary<Vote, int> FinaleLevelEffects = new Dictionary<Vote, int>();
    public Dictionary<Vote, int> NextLevelEffects = new Dictionary<Vote, int>();
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
        if(LevelNumber > vote.LevelNumber) //need to change logic
        {
            foreach (var vote in NextLevelEffects)
            {
                Vote currentVote = vote.Key;
                int choiceIndex = vote.Value;
                currentVote.ApplyEffects(choiceIndex);
            }
            NextLevelEffects.Clear();
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
    public void OnLevelEnd() //need to add a call somewhere
    {
        foreach(PlayerEntity player in PlayerEntity.PlayerList)
        {
            player.CombatManager.ResetDamageTakenMultiplier();
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