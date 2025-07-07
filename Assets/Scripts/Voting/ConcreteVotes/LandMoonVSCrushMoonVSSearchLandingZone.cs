using UnityEngine;

[CreateAssetMenu(fileName = "MoonVote", menuName = "Votes/MoonVote")]
public class LandMoonVSCrushMoonVSSearchLandingZone : Vote
{
    public override int LevelNumber => GameManager.Instance.LevelNumber;
    public override string VoteTitle => "Land on the moon\r\nVS.\r\ncrush on the moon (like a badass)\r\nVS.\r\nSearch for a different landing zone";
    public override string VoteDescription => "After defeating the Alien’s leader, you board their mothership, 10… 9… 8… No time!" +
        " TAKE OFF! You fly sky high and exit the atmosphere, congrats! You escaped Earth, your next destination: The Moon" +
        ", but how will you land?";

    public override string[] Choices => new string[3] { "Land on the moon safely",
        "Crush on the moon (like a badass)", "Search for a different landing zone"};

    public override void ApplyEffects(int i) //add logic later
    {
        switch (i)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }
}
