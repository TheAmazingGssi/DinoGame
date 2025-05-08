using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public GameObject EnemyPrefab;

    [HideInInspector] public float MinSpawnTime;
    [HideInInspector] public float MaxSpawnTime;
    [HideInInspector] public float EnemiesInWave;
    private void Awake()
    {
    }

    private void Start()
    {
        Debug.Log(MinSpawnTime);
        Debug.Log(MaxSpawnTime);
    }
    private void SpawnWave()
    {
        for (int i = 0; i < EnemiesInWave; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        EnemySpawnTime();
        Debug.Log("Enemy spawned");
        Instantiate(EnemyPrefab, gameObject.transform);
    }
    private IEnumerator EnemySpawnTime()
    {
        yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SpawnWave();
        }
    }
}
