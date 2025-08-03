using UnityEngine;

[CreateAssetMenu(fileName = "HelpSaberToothVSStealAcorn", menuName = "Votes/HelpSaberToothVSStealAcorn")]
public class HelpSaberToothVSStealAcorn : Vote
{
    [SerializeField] private int voteOriginLevel = -1;

    public override int LevelNumber => GameManager.Instance.LevelNumber;
    public override string VoteTitle => "Help the saber tooth squirrel\r\nVS.\r\nSteal his acorn";
    public override string VoteDescription => "As you pass the vicious western desert," +
        " you all feel a chill running down your spine, [most score dino] can't shake the feeling you're being watched," +
        " and soon after you find Erl the sabertooth squirrel, he warns you that a blizzard is on its way! He asks you to shelter" +
        " his acorns and promises a great reward if you agree. Or.. you can always steal them for extra nutrition";
    public override string[] Choices => new string[2] { "Shelter the acorn\r\ntake 20% less damage.\n<color=red>BUT</color>\nMore ice meteors on the next level.\r\n",
        "Steal the acorns\r\nraise max health\n<color=red>BUT</color>\nthe highest score player takes more damage next level."};

    public override void ApplyEffects(int i)
    {
        Debug.Log($"SaberTooth ApplyEffects called - Choice: {i}, WasActivated: {wasActivated}, Current Level: {GameManager.Instance.LevelNumber}, VoteOriginLevel: {voteOriginLevel}");

        switch (i)
        {
            case 0: //Shelter the acorn
                if (!wasActivated)
                {
                    Debug.Log("Applying 20% damage reduction to all players");
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.CombatManager.ApplyDamageTakenIncrease(-20);
                    }
                    voteOriginLevel = GameManager.Instance.LevelNumber;
                    VoteEffectManager.Instance.StoreNextLevelEffect(this, i);
                    Debug.Log("Storing next level effect for more ice meteors");
                }
                else
                {
                    if (GameManager.Instance.LevelNumber > voteOriginLevel)
                    {
                        Debug.Log("Applying more ice meteors effect NOW");
                        if (GameManager.Instance.icetroidSpawner != null)
                        {
                            GameManager.Instance.icetroidSpawner.IncreaseIcetroidSpawning(20);
                        }
                        else
                        {
                            Debug.LogWarning("IcetroidSpawner not found in GameManager!");
                        }
                    }
                }
                break;

            case 1: //Steal the acorns
                if (!wasActivated)
                {
                    Debug.Log("Applying health boost to all players");
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.CombatManager.IncreaseMaxHealthByPercentage(20);
                    }
                    voteOriginLevel = GameManager.Instance.LevelNumber;
                    VoteEffectManager.Instance.StoreNextLevelEffect(this, i);
                    Debug.Log("Storing next level effect for damage debuff to highest player");
                }
                else
                {
                    if (GameManager.Instance.LevelNumber > voteOriginLevel)
                    {
                        Debug.Log("Applying damage debuff to highest scoring player NOW");
                        PlayerEntity highest = GameManager.Instance.GetHighestScorePlayer();
                        if (highest)
                        {
                            highest.CombatManager.ApplyDamageTakenIncrease(15);
                            Debug.Log("Damage debuff applied to: " + highest.name);
                        }
                    }
                }
                break;
        }
    }
}