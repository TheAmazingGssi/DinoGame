using System.Collections.Generic;
using UnityEngine;

public class Vote
{
    public string VoteDescription { get; private set; }
    
    public string[] Choices { get; private set; } //maybe later change to dictionary with vote effects

    public Vote(string voteDescription, string[] choices)
    {
        VoteDescription = voteDescription;
        Choices = choices;
    }
}
