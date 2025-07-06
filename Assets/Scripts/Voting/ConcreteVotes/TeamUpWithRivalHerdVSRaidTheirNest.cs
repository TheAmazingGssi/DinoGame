using UnityEngine;

[CreateAssetMenu(fileName = "TeamUpVSRaidNestVote", menuName = "Votes/TeamUpVSRaidNest")]
public class TeamUpWithRivalHerdVSRaidTheirNest : Vote
{
    public override int LevelNumber => GameManager.Instance.LevelNumber;
    public override string VoteTitle => "Gain local alliances for future help\r\nVS.\r\ngaining supply that will help now";
    public override string VoteDescription => "You arrive at the Western Desert, A wasteland that's occupied by bandit-herds" +
        " and crime dino-lords. As you get off your dino-mount, you can't shake the feeling that the bandits around are very" +
        " suspicious of your little herd. Will you try you earn their trust by teaming up with these bandits? Or will you use" +
        " the opportunity to raid their nest? You could use the resources.";

    public override string[] Choices => new string[2] { "Raid their base\r\ngain max health boost.\n<color=red>BUT</color>\nOn the next level, face more enemies\r\n",
        "Team up with Rival Herd:\r\nbecome stronger in the final level\n<color=red>BUT</color>\n[Terry] takes more damage in the final level"};
    public override void ApplyEffects(int i)
    {
        switch (i)
        {
            case 0:
                if(!wasActivated)
                {
                    foreach(PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.CombatManager.IncreaseMaxHealthByPercentage(15);
                    }
                }
                if(LevelNumber == GameManager.Instance.LevelNumber)
                {
                    GameManager.Instance.NextLevelEffects.Add(this, i);

                }
                else
                {
                    foreach (EnemySpawner spawner in GameManager.Instance.SpawnerManager.EnemySpawners)
                    {
                        spawner.EnemiesInWaveMultiplier = 5;
                    }
                }
                break;
            case 1:
                if (!wasActivated)
                {
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.MainPlayerController.ApplyDamageBoost(20);
                    }
                }
                if (GameManager.Instance.LevelNumber == GameManager.Instance.FinaleLevel)
                {
                    PlayerEntity highest = GameManager.Instance.GetHighestScorePlayer();
                    if (highest)
                    {
                        highest.CombatManager.ApplyDamageTakenIncrease(20);
                        Debug.Log("debuff to: " + highest.name);
                    }
                }
                else
                {
                    GameManager.Instance.FinaleLevelEffects.Add(this, i);
                    Debug.Log("adding to finale level votes");
                }
                    break;
        }
    }

}
