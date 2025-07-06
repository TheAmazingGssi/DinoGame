using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private EnemySpawnerData[] data;

    [field: SerializeField] public EnemySpawner[] EnemySpawners { get; private set; }

    [SerializeField] private Collider2D groundCheck;

    public Collider2D GroundCheck => groundCheck;


    private void Awake()
    {
        if (data.Length != EnemySpawners.Length) Debug.LogError("enemy spawners and data not equal!");

        for (int i = 0; i < EnemySpawners.Length; i++)
        {
            EnemySpawners[i].EnemyPrefab = data[i].EnemyPrefab;
            EnemySpawners[i].MinSpawnTime = data[i].MinSpawnTime;
            EnemySpawners[i].MaxSpawnTime = data[i].MaxSpawnTime;
        }
    }
}
