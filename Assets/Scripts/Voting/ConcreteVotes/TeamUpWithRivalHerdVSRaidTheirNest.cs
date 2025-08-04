using UnityEngine;

[CreateAssetMenu(fileName = "TeamUpVSRaidNestVote", menuName = "Votes/TeamUpVSRaidNest")]
public class TeamUpWithRivalHerdVSRaidTheirNest : Vote
{
    [SerializeField] private int voteOriginLevel = -1;
    [SerializeField] private bool nextLevelEffectApplied = false;
    [SerializeField] private bool finaleLevelEffectApplied = false;

    public override int LevelNumber => GameManager.Instance.LevelNumber;
    public override string VoteTitle => "Gain local alliances for future help\r\nVS.\r\ngaining supply that will help now";
    public override string VoteDescription => "You arrive at the Western Desert wastes.\r\nAs you get off your dino-mount, you can't shake the feeling that the local bandits are very suspicious of your little herd. \r\nwill you team up with these bandits? Or will you raid their nest? You could use the resources.";
    public override string[] Choices => new string[2] {
        "Raid their base\r\ngain max health boost.\n<color=red>BUT</color>\nOn the next level, face more enemies\r\n",
        "Team up with Rival Herd:\r\nbecome stronger in the final level\n<color=red>BUT</color>\nThe highest scoring player takes more damage in the final level"
    };

    public override void ApplyEffects(int i)
    {
        Debug.Log($"ApplyEffects called - Choice: {i}, WasActivated: {wasActivated}, Current Level: {GameManager.Instance.LevelNumber}, VoteOriginLevel: {voteOriginLevel}");

        switch (i)
        {
            case 0: //Raid their base
                if (!wasActivated)
                {
                    Debug.Log("Applying health boost to all players");
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        if (player != null && player.CombatManager != null)
                        {
                            player.CombatManager.IncreaseMaxHealthByPercentage(15);
                        }
                    }
                    voteOriginLevel = GameManager.Instance.LevelNumber;
                    VoteEffectManager.Instance.StoreNextLevelEffect(this, i);
                    Debug.Log("Storing next level effect for enemy increase");
                }
                else
                {
                    if (GameManager.Instance.LevelNumber == voteOriginLevel + 1 && !nextLevelEffectApplied)
                    {
                        Debug.Log("Applying enemy increase effect NOW (one time only)");
                        GameManager.Instance.SpawnerManager.IncreaseEnemies(3);
                        nextLevelEffectApplied = true;
                    }
                }
                break;

            case 1: //Team up
                if (!wasActivated)
                {
                    Debug.Log("Applying damage boost to all players");
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        if (player != null && player.MainPlayerController != null)
                        {
                            player.MainPlayerController.ApplyDamageBoost(20);
                        }
                    }

                    if (GameManager.Instance.LevelNumber != GameManager.Instance.FinaleLevel)
                    {
                        VoteEffectManager.Instance.StoreFinaleLevelEffect(this, i);
                        Debug.Log("Storing finale level effect");
                    }
                }
                else
                {
                    if (GameManager.Instance.LevelNumber == GameManager.Instance.FinaleLevel && !finaleLevelEffectApplied)
                    {
                        Debug.Log("Applying finale level debuff NOW (one time only)");
                        PlayerEntity highest = GameManager.Instance.GetHighestScorePlayer();
                        if (highest != null && highest.CombatManager != null)
                        {
                            highest.CombatManager.ApplyDamageTakenIncrease(20);
                            Debug.Log("debuff to: " + (highest.gameObject != null ? highest.name : "Destroyed Player"));
                        }
                        finaleLevelEffectApplied = true;
                    }
                }
                break;
        }
    }
}
