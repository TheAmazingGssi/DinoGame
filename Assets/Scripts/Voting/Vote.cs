using System.Collections.Generic;
using UnityEngine;

public abstract class Vote : ScriptableObject
{
    public bool wasActivated = false;
    public abstract int LevelNumber { get; }
    public abstract string VoteTitle { get; }
    public abstract string VoteDescription { get; }
    public abstract string[] Choices { get; }
    public abstract void ApplyEffects(int i);
}