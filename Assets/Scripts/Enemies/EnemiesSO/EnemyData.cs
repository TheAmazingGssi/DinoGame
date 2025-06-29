using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float maxHealth;

    [SerializeField] private float baseDamage;

    [Tooltip("Distance from the player where the enemy should stop moving")]
    [SerializeField] private float stopRange;

    [SerializeField] private float attackRange;

    [SerializeField] private float speed;


    public float Speed => speed;
    public float AttackRange => attackRange;
    public float StopRange => stopRange;
    public float MaxHealth => maxHealth;
    public float BaseDamage => baseDamage;
}
