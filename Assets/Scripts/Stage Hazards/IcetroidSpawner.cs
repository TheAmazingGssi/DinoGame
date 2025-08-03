using System;
using System.Collections;
using UnityEngine;

public class IcetroidSpawner : MonoBehaviour
{
    [Header("Icetroid Settings")]
    [SerializeField] private GameObject icetroidPrefab;
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private int baseMaxIcetroids = 5;

    [Header("Spawn Timing")]
    [SerializeField] private float baseMinSpawnInterval = 1f;
    [SerializeField] private float baseMaxSpawnInterval = 3f;

    private int currentMaxIcetroids;
    private float currentMinSpawnInterval;
    private float currentMaxSpawnInterval;

    private float nextSpawnTime;
    private int activeIcetroids = 0;

    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        leftEdge = spawnArea.bounds.min.x;
        rightEdge = spawnArea.bounds.max.x;

        ResetToBaseValues();

        Debug.Log($"IcetroidSpawner initialized - Max: {currentMaxIcetroids}, Intervals: {currentMinSpawnInterval}-{currentMaxSpawnInterval}");
    }

    private void Start()
    {
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime && activeIcetroids < currentMaxIcetroids)
        {
            SpawnIcetroid();
            SetNextSpawnTime();
        }
    }

    public void IncreaseIcetroidSpawning(int percentageIncrease)
    {
        Debug.Log($"IncreaseIcetroidSpawning called with {percentageIncrease}% increase");

        int oldMax = currentMaxIcetroids;
        currentMaxIcetroids = Mathf.RoundToInt(baseMaxIcetroids * (1 + percentageIncrease / 100f));

        float speedMultiplier = 1f - (percentageIncrease / 200f);
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.3f, 1f);

        float oldMinInterval = currentMinSpawnInterval;
        float oldMaxInterval = currentMaxSpawnInterval;

        currentMinSpawnInterval = baseMinSpawnInterval * speedMultiplier;
        currentMaxSpawnInterval = baseMaxSpawnInterval * speedMultiplier;

        Debug.Log($"Icetroid spawning increased: Max {oldMax} -> {currentMaxIcetroids}, " +
                  $"Intervals {oldMinInterval:F1}-{oldMaxInterval:F1} -> {currentMinSpawnInterval:F1}-{currentMaxSpawnInterval:F1}");
    }

    public void ResetToBaseValues()
    {
        currentMaxIcetroids = baseMaxIcetroids;
        currentMinSpawnInterval = baseMinSpawnInterval;
        currentMaxSpawnInterval = baseMaxSpawnInterval;
    }

    private void SpawnIcetroid()
    {
        float spawnX = UnityEngine.Random.Range(leftEdge, rightEdge);
        float spawnY = transform.position.y;
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        GameObject icetroidObj = Instantiate(icetroidPrefab, spawnPosition, Quaternion.identity);
        activeIcetroids++;

        Icetroid icetroidScript = icetroidObj.GetComponent<Icetroid>();
        if (icetroidScript != null)
        {
            icetroidScript.OnDestroyed += HandleIcetroidDestroyed;
        }
    }

    private void HandleIcetroidDestroyed()
    {
        activeIcetroids--;
    }

    private void SetNextSpawnTime()
    {
        float randomInterval = UnityEngine.Random.Range(currentMinSpawnInterval, currentMaxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }
}