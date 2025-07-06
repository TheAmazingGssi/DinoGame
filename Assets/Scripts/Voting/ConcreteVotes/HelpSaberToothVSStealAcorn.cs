using UnityEngine;

[CreateAssetMenu(fileName = "HelpSaberToothVSStealAcorn", menuName = "Votes/HelpSaberToothVSStealAcorn")]

public class HelpSaberToothVSStealAcorn : Vote
{
    public override int LevelNumber => GameManager.Instance.LevelNumber;
    public override string VoteTitle => "Help the saber tooth squirrel\r\nVS.\r\nSteal his acorn";
    public override string VoteDescription => "Help the saber tooth squirrel\r\nVS.\r\nSteal his acorn";

    public override string[] Choices => new string[2] { "Shelter the acorn\r\ntake 20% less damage.\n<color=red>BUT</color>\nMore ice meteors on the next level.\r\n",
        "Steal the acorns\r\nraise max health\n<color=red>BUT</color>\nthe highest score player takes more damage next level."};
    public override string[] ButtonTexts => new string[2] { "Shelter the acorn", "Steal the acorns" };

    public override void ApplyEffects(int i)
    {
        switch (i)
        {
            case 0:
                if (!wasActivated)
                {
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.CombatManager.ApplyDamageTakenIncrease(-20);
                    }
                }
                if (LevelNumber == GameManager.Instance.LevelNumber)
                {
                    GameManager.Instance.NextLevelEffects.Add(this, i);

                }
                else
                {
                    //add 20% more ice meteors
                }
                break;
            case 1:
                if (!wasActivated)
                {
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.CombatManager.IncreaseMaxHealthByPercentage(20);
                    }
                }
                if (LevelNumber == GameManager.Instance.LevelNumber)
                {
                    GameManager.Instance.NextLevelEffects.Add(this, i);

                }
                else
                {
                    PlayerEntity highest = GameManager.Instance.GetHighestScorePlayer();
                    if (highest)
                    {
                        highest.CombatManager.ApplyDamageTakenIncrease(15);
                        Debug.Log("debuff to: " + highest.name);
                    }
                }
                break;
        }
        wasActivated = true;
    }
}
