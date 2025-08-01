using UnityEngine;

[CreateAssetMenu(fileName = "HelpSaberToothVSStealAcorn", menuName = "Votes/HelpSaberToothVSStealAcorn")]

public class HelpSaberToothVSStealAcorn : Vote
{
    public override int LevelNumber => GameManager.Instance.LevelNumber;
    public override string VoteTitle => "Help the saber tooth squirrel\r\nVS.\r\nSteal his acorn";
    public override string VoteDescription => "As you pass the vicious western desert," +
        " you all feel a chill running down your spine, [most score dino] can't shake the feeling you’re being watched," +
        " and soon after you find Erl the sabertooth squirrel, he warns you that a blizzard is on its way! He asks you to shelter" +
        " his acorns and promises a great reward if you agree. Or.. you can always steal them for extra nutrition";

    public override string[] Choices => new string[2] { "Shelter the acorn\r\ntake 20% less damage.\n<color=red>BUT</color>\nMore ice meteors on the next level.\r\n",
        "Steal the acorns\r\nraise max health\n<color=red>BUT</color>\nthe highest score player takes more damage next level."};

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
                        Debug.Log("Applying 20% less damage effect case 0 saber tooth");
                    }
                }
                if (LevelNumber == GameManager.Instance.LevelNumber)
                {
                    GameManager.Instance.NextLevelEffects.Add(this, i);
                    Debug.Log("adding case 0 choice to next level effect, saber tooth");

                }
                else
                {
                    //add 20% more ice meteors
                    Debug.Log("more ice meteors");
                }
                break;
            case 1:
                if (!wasActivated)
                {
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.CombatManager.IncreaseMaxHealthByPercentage(20);
                        Debug.Log("case 1 increasing health");
                    }
                }
                if (LevelNumber == GameManager.Instance.LevelNumber)
                {
                    GameManager.Instance.NextLevelEffects.Add(this, i);
                    Debug.Log("adding to next level case 1");

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
