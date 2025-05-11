using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public GameObject EnemyPrefab;

    [HideInInspector] public float MinSpawnTime;
    [HideInInspector] public float MaxSpawnTime;
    [HideInInspector] public float EnemiesInWave;

    private bool wasTriggered = false;
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < EnemiesInWave; i++)
        {
            Instantiate(EnemyPrefab, transform);
            yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(!wasTriggered)
            {
                StartCoroutine(SpawnWave());
                wasTriggered = true;
            }
        }
    }
}
