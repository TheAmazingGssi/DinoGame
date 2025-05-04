using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float maxHealth;

    [SerializeField] private float baseDamage;

    [SerializeField] private float cooldown;

    [SerializeField] private float attackRange;

    [SerializeField] private float detectionRange;

    [SerializeField] private float speed;


    public float Speed => speed;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public float Cooldown => cooldown;
    public float MaxHealth => maxHealth;
    public float BaseDamage => baseDamage;
}
