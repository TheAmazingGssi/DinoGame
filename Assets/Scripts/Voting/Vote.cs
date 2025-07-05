using System.Collections.Generic;
using UnityEngine;
public enum TimeOfEffect
{
    Immediate,
    OnlyNextLevel,
    FinaleLevel
}
public abstract class Vote : MonoBehaviour
{
    protected bool wasActivated = false;
    public abstract string VoteDescription { get;}

    public abstract string[] Choices { get;}
    public abstract string[] ButtonTexts { get;}

    public abstract void ApplyEffects(int i);
}
