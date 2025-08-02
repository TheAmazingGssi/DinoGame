using System;
using System.Collections;
using UnityEngine;

public class IcetroidSpawner : MonoBehaviour
{
    [Header("Icetroid Settings")]
    [SerializeField] private GameObject icetroidPrefab;
    [SerializeField] private int maxIcetrods = 5;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;

    [Header("Spawn Positioning")]
    [SerializeField] private float spawnHeight = 5f;
    [SerializeField] private float spawnRangeX = 10f;

    private float nextSpawnTime;
    private int currentIcetrods = 0;


    private void Start()
    {
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime && currentIcetrods < maxIcetrods)
        {
            SpawnIcetroid();
            SetNextSpawnTime();
        }
    }

    private void SpawnIcetroid()
    {
        float spawnX = UnityEngine.Random.Range(-spawnRangeX, spawnRangeX);
        float spawnY = transform.position.y;
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        Instantiate(icetroidPrefab, spawnPosition, Quaternion.identity);
    }

    private void SetNextSpawnTime()
    {
        float randomInterval = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }
}