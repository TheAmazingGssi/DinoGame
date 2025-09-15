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
    
    [Header("Burst Settings")]
    [SerializeField] [Min(1)] private int defaultBurstCount = 3;
    [SerializeField] [Min(0f)] private float defaultBurstInterval = 0.15f;
    [SerializeField] private bool disableNormalSpawningDuringBurst = true;

    private bool normalSpawningEnabled = true;
    private bool isBursting = false;
    private Coroutine burstRoutine = null;

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
        if ((disableNormalSpawningDuringBurst && isBursting) || !normalSpawningEnabled)
            return;

        if (Time.time >= nextSpawnTime && activeIcetroids < currentMaxIcetroids)
        {
            SpawnIcetroid();
            SetNextSpawnTime();
        }
    }
    
    
    // Triggers a burst of spawns.
    public void TriggerBurst(int count = -1, float interval = -1f)
    {
        int burstCount = (count > 0) ? count : defaultBurstCount;
        float burstInterval = (interval >= 0f) ? interval : defaultBurstInterval;

        if (burstRoutine != null)
            StopCoroutine(burstRoutine);

        burstRoutine = StartCoroutine(BurstSequence(burstCount, burstInterval));
    }

    
    private IEnumerator BurstSequence(int count, float interval)
    {
        yield return new WaitForSeconds(0.25f);
        
        isBursting = true;

        for (int i = 0; i < count; i++)
        {
            if (activeIcetroids < currentMaxIcetroids)
                SpawnIcetroid();

            if (interval <= 0f)
                yield return null;
            else
                yield return new WaitForSeconds(interval);
        }

        isBursting = false;
        burstRoutine = null;

        // Reset normal spawning so it doesn’t get stuck
        SetNextSpawnTime();
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
    }

    public void ResetToBaseValues()
    {
        currentMaxIcetroids = baseMaxIcetroids;
        currentMinSpawnInterval = baseMinSpawnInterval;
        currentMaxSpawnInterval = baseMaxSpawnInterval;
    }

    private void SpawnIcetroid()
    {
        leftEdge = spawnArea.bounds.min.x;
        rightEdge = spawnArea.bounds.max.x;
        
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