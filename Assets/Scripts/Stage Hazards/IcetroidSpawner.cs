using System;
using System.Collections;
using UnityEngine;

public class IcetroidSpawner : MonoBehaviour
{
    [Header("Icetroid Settings")]
    [SerializeField] private GameObject icetroidPrefab;
    [SerializeField] private Collider2D spawnArea;

    [SerializeField] private int maxIcetrods = 5;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;
    
    private float nextSpawnTime;
    private int currentIcetrods = 0;
    
    // Edges of the spawn area
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        leftEdge = spawnArea.bounds.min.x;
        rightEdge = spawnArea.bounds.max.x;
    }

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
        float spawnX = UnityEngine.Random.Range(leftEdge, rightEdge);
        float spawnY = transform.position.y;
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        GameObject icetroidObj = Instantiate(icetroidPrefab, spawnPosition, Quaternion.identity);

        currentIcetrods++;

        Icetroid icetroidScript = icetroidObj.GetComponent<Icetroid>();
        
        if (icetroidScript != null)
        {
            icetroidScript.OnDestroyed += HandleIcetroidDestroyed;
        }
    }

    private void HandleIcetroidDestroyed()
    {
        currentIcetrods--;
    }

    private void SetNextSpawnTime()
    {
        float randomInterval = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }
}