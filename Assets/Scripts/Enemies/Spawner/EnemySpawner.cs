using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    private const string Player = "Player";
    [HideInInspector] public GameObject EnemyPrefab;
    [HideInInspector] public float MinSpawnTime;
    [HideInInspector] public float MaxSpawnTime;
    public int EnemiesInWaveMultiplier = 2;
    private int EnemiesInWave;
    [SerializeField] Transform[] spawnPoints;
    private bool wasTriggered = false;
    public UnityEvent WaveStart;

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < EnemiesInWave; i++)
        {
            Instantiate(EnemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            if (!wasTriggered)
            {
                WaveStart.Invoke();
                if (EnemiesInWaveMultiplier == -1) EnemiesInWave = 1;
                else EnemiesInWave = PlayerEntity.PlayerList.Count * EnemiesInWaveMultiplier;
                GameManager.Instance.SetWaveSize(EnemiesInWave);
                StartCoroutine(SpawnWave());
                wasTriggered = true;
            }
        }
    }
}