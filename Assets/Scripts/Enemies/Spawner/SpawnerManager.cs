using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private EnemySpawnerData[] data;

    [SerializeField] private EnemySpawner[] enemySpawners;

    private void Awake()
    {
        if (data.Length != enemySpawners.Length) Debug.LogError("enemy spawners and data not equal!");

        for (int i = 0; i < enemySpawners.Length; i++)
        {
            enemySpawners[i].EnemyPrefab = data[i].EnemyPrefab;
            enemySpawners[i].MinSpawnTime = data[i].MinSpawnTime;
            enemySpawners[i].MaxSpawnTime = data[i].MaxSpawnTime;
            enemySpawners[i].EnemiesInWave = data[i].EnemiesInWave;
        }
    }
}
