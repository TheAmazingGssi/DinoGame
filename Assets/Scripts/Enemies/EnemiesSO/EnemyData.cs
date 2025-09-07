using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public float MaxHealth {get; private set;}

    [field: SerializeField] public float BaseDamage { get; private set; }

    [Tooltip("Distance from the player where the enemy should stop moving")]
    [field: SerializeField] public float StopRange { get; private set; }

    [field: SerializeField] public float AttackRange { get; private set; }

    [field: SerializeField] public float Speed { get; private set; }

    [field: SerializeField] public int Score { get; private set; }

    [field: SerializeField] public bool ImmuneToKnockback { get; private set; }
    [field: SerializeField] public bool IsInterruptible { get; private set; } = true;
}
