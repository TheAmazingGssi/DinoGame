using System.Collections.Generic;
using UnityEngine;

public enum EffectedPlayers
{
    HighestScore,
    LowestScore,
    All,
    none
}

public abstract class VoteEffectBase
{
    public abstract TimeOfEffect timeOfEffect {  get; }
    public abstract EffectedPlayers effectedPlayers {  get; }

    abstract public void ApplyEffect(List<PlayerEntity> players);
}
