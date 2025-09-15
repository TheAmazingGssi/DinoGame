using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
// ---------- Singleton & Static Events ----------
    public static GameManager Instance;
    public static System.Action<MainPlayerController> OnLevelEnd;

// ---------- Instance Events ----------
    public event UnityEngine.Events.UnityAction<MainPlayerController> OnScoreChange;
    [SerializeField] private UnityEngine.Events.UnityEvent waveCompleted = new UnityEngine.Events.UnityEvent();


// =====================  INSPECTOR: SETTINGS  =====================
    [Header("Settings ▸ Level & Waves")]
    [SerializeField] private CameraLocations[] waveLocations;
    [SerializeField] private Vote vote;
    [field: SerializeField] public int LevelNumber { get; private set; } = 1;
    public float VotePersistenceDuration = 10f;
    public int FinaleLevel = 3;


// =====================  INSPECTOR: REFERENCES  =====================
    [Header("References")]
    [SerializeField] private VotingManager stagesVote;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private EndLevelSpotlight endLevelSpotlight;
    [field: SerializeField] public SpawnerManager SpawnerManager { get; private set; }
    public IcetroidSpawner icetroidSpawner;


// =====================  RUNTIME STATE  =====================
    [Header("State ▸ Voting & Effects (Runtime)")]
    public Dictionary<Vote, int> FinaleLevelEffects = new Dictionary<Vote, int>();
    public Dictionary<Vote, int> NextLevelEffects   = new Dictionary<Vote, int>();
    public bool IsInCoopAttack = false;

    [Header("State ▸ Counters & Collections (Runtime)")]
    private int enemiesOnStage = 0;
    private int currentWave    = 0;
    public List<EnemyManager> ActiveEnemies;


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
            sceneLoader.LoadScene(Scenes.LoseScreen);

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
        if (IsInCoopAttack)
        {
            StartCoroutine(PostponedLevelEnd());
            return;
        }
        
        
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            player.CombatManager.ResetDamageTakenMultiplier();
        }
        
        MainPlayerController highestScorePlayer = GetHighestScorePlayer().MainPlayerController;
        OnLevelEnd?.Invoke(highestScorePlayer);
        StartCoroutine(VictoryMoment());
        PlayerEntity.SaveScore();
        endLevelSpotlight.EnableDark();
    }
    
    private IEnumerator PostponedLevelEnd()
    {
        yield return new WaitForSeconds(3);
        LevelEnd();
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
        endLevelSpotlight.DisableDark();
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

    public void HandleScoreChange()
    {
        bool startOfGame = true;
        foreach (var player in PlayerEntity.PlayerList)
        {
            if (player.MainPlayerController == null) return;
            if (player.MainPlayerController.GetScore() != 0) startOfGame = false;
        }
        if (!startOfGame)
            OnScoreChange?.Invoke(GetHighestScorePlayer().MainPlayerController);
    }
}

[System.Serializable]
struct CameraLocations
{
    public int LeftMost;
    public int RightMost;
}