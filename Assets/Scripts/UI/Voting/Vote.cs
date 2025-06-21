using System.Collections.Generic;
using UnityEngine;

public class Vote
{
    public string VoteDescription { get; private set; }
    
    public string[] Choices { get; private set; } 
    public string[] ButtonTexts { get; private set; } //maybe later change to dictionary with vote effects

    public Vote(string voteDescription, string[] choices, string[] buttonTexts)
    {
        VoteDescription = voteDescription;
        Choices = choices;
        ButtonTexts = buttonTexts;
    }
}
