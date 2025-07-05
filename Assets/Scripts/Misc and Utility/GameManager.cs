using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] VotingManager stagesVote;
    [SerializeField] Vote vote;

    public static GameManager Instance;
    private int enemiesOnStage = 0;

    void Start()
    {
        if (!Instance && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        VotingManager.OnVoteComplete += HandleVoteComplete;
    }

    private void OnDisable()
    {
        VotingManager.OnVoteComplete -= HandleVoteComplete;
    }

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
        switch (vote.Effects[winningChoice].timeOfEffect)
        {
            case TimeOfEffect.Immediate:
                ApplyEffectsToPlayers(winningChoice);
                break;
            case TimeOfEffect.OnlyNextLevel:
                break;
            case TimeOfEffect.FinaleLevel:
                break;
        }
    }

    private void ApplyEffectsToPlayers(int i)
    {
        switch (vote.Effects[i].effectedPlayers)
        {
            case EffectedPlayers.All:
                vote.Effects[i].ApplyEffect(PlayerEntity.PlayerList);
                break;

            case EffectedPlayers.LowestScore:
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
                vote.Effects[i].ApplyEffect(new List<PlayerEntity> { lowestScorePlayer });
                break;

            case EffectedPlayers.HighestScore:
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
                vote.Effects[i].ApplyEffect(new List<PlayerEntity> { highestScorePlayer });
                break;

            case EffectedPlayers.none:
                vote.Effects[i].ApplyEffect(new List<PlayerEntity> {});
                break;
        }
    }
}
