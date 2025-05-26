using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerData", menuName = "Scriptable Objects/EnemySpawnerData")]
public class EnemySpawnerData : ScriptableObject
{
    [SerializeField] private GameObject enemyPrefab;
    public GameObject EnemyPrefab => enemyPrefab;

    [SerializeField] private float minSpawnTime;
    public float MinSpawnTime => minSpawnTime;

    [SerializeField] private float maxSpawnTime;
    public float MaxSpawnTime => maxSpawnTime;
}
