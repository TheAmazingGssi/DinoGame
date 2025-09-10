using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoarWaveBurstSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    [SerializeField] private WaveProjectileFromPool wavePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Player Reference")]
    [Tooltip("MainPlayerController of the character that owns this spawner (used to read facing).")]
    [SerializeField] private MainPlayerController mainPlayerController;  

    [Header("Pool")]
    [SerializeField, Min(1)] private int initialPoolSize = 8;

    [Header("Wave Motion")]
    [SerializeField, Min(0.1f)] private float waveSpeed = 12f;
    [SerializeField, Min(0.1f)] private float waveMaxDistance = 8f;

    [Header("Burst")]
    [SerializeField, Min(1)] private int wavesPerBurst = 4;
    [SerializeField, Min(0f)] private float waveIntervalSeconds = 0.15f;

    [Header("Direction (fallback only)")]
    [Tooltip("Only used if mainPlayerController is not set.")]
    [SerializeField] private Vector2 customDirection = Vector2.right;

    // pool
    private readonly Queue<WaveProjectileFromPool> pool = new Queue<WaveProjectileFromPool>();
    private Coroutine burstRoutine;

    private void Awake()
    {
        if (wavePrefab == null)
        {
            Debug.LogError($"{nameof(RoarWaveBurstSpawner)}: wavePrefab is not assigned.");
            enabled = false;
            return;
        }

        if (spawnPoint == null)
            spawnPoint = transform;

        for (int i = 0; i < initialPoolSize; i++)
            pool.Enqueue(CreateNewWave());
    }

    private Vector2 GetDirection()
    {
        return mainPlayerController.IsFacingRight? Vector2.right : Vector2.left;
    }


    public void TriggerBurst()
    {
        Vector2 dir = GetDirection();
        StartBurst(wavesPerBurst, waveIntervalSeconds, dir);
    }


    public void TriggerBurst(int count, float intervalSeconds)
    {
        Vector2 dir = GetDirection();
        StartBurst(Mathf.Max(1, count), Mathf.Max(0f, intervalSeconds), dir);
    }


    public void TriggerBurst(int count, float intervalSeconds, Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
        StartBurst(Mathf.Max(1, count), Mathf.Max(0f, intervalSeconds), direction.normalized);
    }


    public void SpawnWaveAnimationEvent()
    {
        FireSingle(GetDirection());
    }
    

    private void StartBurst(int count, float intervalSeconds, Vector2 direction)
    {
        if (burstRoutine != null)
            StopCoroutine(burstRoutine);
        burstRoutine = StartCoroutine(BurstRoutine(count, intervalSeconds, direction));
    }

    private IEnumerator BurstRoutine(int count, float intervalSeconds, Vector2 direction)
    {
        for (int i = 0; i < count; i++)
        {
            FireSingle(direction);
            if (i < count - 1 && intervalSeconds > 0f)
                yield return new WaitForSeconds(intervalSeconds);
        }
        burstRoutine = null;
    }

    private void FireSingle(Vector2 direction)
    {
        WaveProjectileFromPool wave = GetFromPool();
        wave.transform.position = spawnPoint.position;

        // rotate for 2D visuals, motion uses the direction vector
        float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        wave.transform.rotation = Quaternion.Euler(0f, 0f, z);

        wave.gameObject.SetActive(true);
        wave.Launch(direction, waveSpeed, waveMaxDistance, ReturnToPool);
    }
    

    private WaveProjectileFromPool GetFromPool()
    {
        if (pool.Count == 0)
            pool.Enqueue(CreateNewWave());
        return pool.Dequeue();
    }

    private void ReturnToPool(WaveProjectileFromPool wave)
    {
        wave.gameObject.SetActive(false);
        pool.Enqueue(wave);
    }

    private WaveProjectileFromPool CreateNewWave()
    {
        WaveProjectileFromPool wave = Instantiate(wavePrefab, transform);
        wave.gameObject.SetActive(false);
        return wave;
    }
}
