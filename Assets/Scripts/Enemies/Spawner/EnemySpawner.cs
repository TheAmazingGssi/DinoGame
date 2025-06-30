using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const string Player = "Player";

    [HideInInspector] public GameObject EnemyPrefab;

    [HideInInspector] public float MinSpawnTime;
    [HideInInspector] public float MaxSpawnTime;
    private float EnemiesInWave;

    [SerializeField] Transform[] spawnPoints;

    private bool wasTriggered = false;

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < EnemiesInWave; i++)
        {
            Instantiate(EnemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)]);
            yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if(collision.CompareTag(Player))
        {
            if(!wasTriggered)
            {
                EnemiesInWave = PlayerEntity.PlayerList.Count * 2;
                StartCoroutine(SpawnWave());
                wasTriggered = true;
            }
        }
    }
}
