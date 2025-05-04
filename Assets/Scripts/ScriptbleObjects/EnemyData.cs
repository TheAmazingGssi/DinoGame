using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float maxHealth;

    [SerializeField] private float baseDamage;

    [SerializeField] private float cooldown;

    [SerializeField] private float range;

    public float Range => range;
    public float Cooldown => cooldown;

    public float MaxHealth => maxHealth;

    public float BaseDamage => baseDamage;
}
