using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TeamUpWithRivalHerdVSRaidTheirNest : Vote
{
    public override string VoteDescription => "Gain local alliances for future help\r\nVS.\r\ngaining supply that will help now";

    public override string[] Choices => new string[2] { "Raid their base\r\ngain max health boost.\n<color=red>BUT</color>\nOn the next level, face more enemies\r\n",
        "Team up with Rival Herd:\r\nbecome stronger in the final level\n<color=red>BUT</color>\n[Terry] takes more damage in the final level"};
    public override string[] ButtonTexts => new string[2] { "Raid their base", "Team up with rival herd" };

    public override void ApplyEffects(int i)
    {
        switch (i)
        {
            case 0:
                break;
            case 1:
                if (!wasActivated)
                {
                    foreach (PlayerEntity player in PlayerEntity.PlayerList)
                    {
                        player.MainPlayerController.ApplyDamageBoost(20);
                    }
                }
                break;
        }
        wasActivated = true;
    }

}
