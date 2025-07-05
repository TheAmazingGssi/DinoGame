using System.Collections.Generic;
using UnityEngine;

public class TeamUpWithRivalHerd : VoteEffectBase
{
    //public override TimeOfEffect timeOfEffect => TimeOfEffect.FinaleLevel;

    public override EffectedPlayers effectedPlayers => EffectedPlayers.All;

    public override void ApplyEffect(List<PlayerEntity> players)
    {
        foreach (PlayerEntity player in players)
        {
            player.MainPlayerController.ApplyDamageBoost(20);
        }
    }
}
