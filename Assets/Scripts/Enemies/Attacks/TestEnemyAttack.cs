using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAttack : EnemyAttack
{

    protected override float AttackRange => throw new System.NotImplementedException();

    protected override bool IsPlayerInRange => throw new System.NotImplementedException();

    protected override void ApplyDamage()
    {
    }
}