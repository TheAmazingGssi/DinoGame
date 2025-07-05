using System.Collections.Generic;
using UnityEngine;

public abstract class Vote : ScriptableObject
{
    protected bool wasActivated = false;

    public abstract string VoteDescription { get; }
    public abstract string[] Choices { get; }
    public abstract string[] ButtonTexts { get; }
    public abstract void ApplyEffects(int i);
}