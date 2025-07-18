using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    public static System.Action<MainPlayerController> OnLevelEnd;

    [Header("Settings")]
    [SerializeField] private CameraLocations[] waveLocations;
    [SerializeField] private Vote vote;
    [field: SerializeField] public int LevelNumber { get; private set; } = 1;
    
    [Header("Refrences")]
    [SerializeField] private VotingManager stagesVote;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private SceneLoader sceneLoader;
    [field:SerializeField] public SpawnerManager SpawnerManager {get; private set;}
    
    public int FinaleLevel = 3;

    //Variables off inspector
    public Dictionary<Vote, int> FinaleLevelEffects = new Dictionary<Vote, int>();
    public Dictionary<Vote, int> NextLevelEffects = new Dictionary<Vote, int>();
    private int enemiesOnStage = 0;
    private int currentWave = 0;

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

        cameraMovement.FurthestLeftPoint = waveLocations[currentWave].LeftMost;
        cameraMovement.FurthestRightPoint = waveLocations[currentWave].RightMost;
    }

    private void OnEnable() => VotingManager.OnVoteComplete += HandleVoteComplete;
    private void OnDisable() => VotingManager.OnVoteComplete -= HandleVoteComplete;

    void Update()
    {
        if (currentWave >= waveLocations.Length)
            return;
        float cameraX = cameraMovement.transform.position.x;
        if (cameraX < waveLocations[currentWave].LeftMost && cameraX > cameraMovement.FurthestLeftPoint)
            cameraMovement.FurthestLeftPoint = cameraX;
    }

    public void IncrementDeathCount()
    {
        enemiesOnStage--;
        if (enemiesOnStage <= 0)
        {
            WaveComplete();
        }
    }

    [ContextMenu ("End Level")]
    public void LevelEnd() //need to add a call somewhere
    {
        foreach(PlayerEntity player in PlayerEntity.PlayerList)
        {
            player.CombatManager.ResetDamageTakenMultiplier();
        }
        MainPlayerController highestScorePlayer = GetHighestScorePlayer().MainPlayerController;
        OnLevelEnd?.Invoke(highestScorePlayer);
    }
    public void SetWaveSize(int amount)
    {
        enemiesOnStage += amount;
    }

    private void WaveComplete()
    {
        currentWave++;
        if (waveLocations.Length <= currentWave)
            LevelEnd();
        else
        {
            cameraMovement.FurthestRightPoint = waveLocations[currentWave].RightMost;
        }
    }

    public void StartVote()
    {
        cameraMovement.FurthestRightPoint = cameraMovement.transform.position.x;
        cameraMovement.FurthestLeftPoint = cameraMovement.transform.position.x;
        vote.wasActivated = false;
        stagesVote.StartVote(vote);
    }

    private void HandleVoteComplete(int winningChoice)
    {
        vote.ApplyEffects(winningChoice);
        vote.wasActivated = true;
        sceneLoader.LoadTargetScene();
    }

    public PlayerEntity GetHighestScorePlayer()
    {
        PlayerEntity highestScorePlayer = null;
        int highestScore = 0;
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

[System.Serializable]
struct CameraLocations
{
    public int LeftMost;
    public int RightMost;
}