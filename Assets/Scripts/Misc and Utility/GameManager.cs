using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public IcetroidSpawner icetroidSpawner;
    [field: SerializeField] public SpawnerManager SpawnerManager { get; private set; }

    public int FinaleLevel = 3;
    public int playerIdCounter = 0;

    //Variables off inspector
    public Dictionary<Vote, int> FinaleLevelEffects = new Dictionary<Vote, int>();
    public Dictionary<Vote, int> NextLevelEffects = new Dictionary<Vote, int>();
    private int enemiesOnStage = 0;
    private int currentWave = 0;
    [SerializeField] UnityEvent waveCompleted = new UnityEvent();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        cameraMovement.FurthestLeftPoint = waveLocations[currentWave].LeftMost;
        cameraMovement.FurthestRightPoint = waveLocations[currentWave].RightMost; 

        StartCoroutine(WaitAndInitialize());
    }

    private IEnumerator WaitAndInitialize()
    {
        yield return new WaitUntil(AreAllPlayersReady);

        VoteEffectManager.Instance.ApplyStoredEffects(LevelNumber, FinaleLevel);
    }

    private bool AreAllPlayersReady()
    {
        if (PlayerEntity.PlayerList.Count == 0) return false;

        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            if (player?.CombatManager == null) return false;
        }

        return true;
    }

    private void OnEnable() => VotingManager.OnVoteComplete += HandleVoteComplete;
    private void OnDisable() => VotingManager.OnVoteComplete -= HandleVoteComplete;

    void Update()
    {
        //check lose condition
        int counter = 0;
        for (int i = 0; i < PlayerEntity.PlayerList.Count; i++)
            if (PlayerEntity.PlayerList[i].CombatManager.CurrentHealth <= 0)
                counter++;
        if (counter == PlayerEntity.PlayerList.Count)
            sceneLoader.LoadScene(Scenes.MainMenu);

        //make camera move to vote animation
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

    [ContextMenu("End Level")]
    public void LevelEnd() //need to add a call somewhere
    {
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            player.CombatManager.ResetDamageTakenMultiplier();
        }
        MainPlayerController highestScorePlayer = GetHighestScorePlayer().MainPlayerController;
        OnLevelEnd?.Invoke(highestScorePlayer);
        StartCoroutine(VictoryMoment());
        PlayerEntity.SaveScore();
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
            waveCompleted.Invoke();
        }
    }

    public void StartVote()
    {
        cameraMovement.FurthestRightPoint = cameraMovement.transform.position.x;
        cameraMovement.FurthestLeftPoint = cameraMovement.transform.position.x;
        vote.wasActivated = false;
        stagesVote.StartVote(vote);
    }

    private IEnumerator VictoryMoment()
    {
        yield return new WaitForSeconds(5);
        StartVote();
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
        highestScorePlayer = highestScorePlayer ?? PlayerEntity.PlayerList[0];
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
        lowestScorePlayer = lowestScorePlayer ?? PlayerEntity.PlayerList[0];
        return lowestScorePlayer;
    }
}

[System.Serializable]
struct CameraLocations
{
    public int LeftMost;
    public int RightMost;
}