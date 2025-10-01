using System.Collections;
using UnityEngine;

public class LegBurstSpawner : MonoBehaviour
{
    [Header("Spawn Target")]
    [SerializeField] private GameObject legPrefab;  // MUST have OscillatingLeg component
    [SerializeField] private Collider2D spawnArea;

    [Header("Burst Settings")]
    [SerializeField, Min(1)] private int defaultBurstCount = 5;
    [SerializeField, Min(0f)] private float defaultBetweenSpawnDelay = 0.1f;

    [Tooltip("If true and count > 1, first spawn is at far left and last at far right. If false, positions are centered (no exact edges).")]
    [SerializeField] private bool includeEdges = true;

    [Header("Randomization")]
    [Tooltip("Randomly jitter each X while guaranteeing minimum spacing between neighbors.")]
    [SerializeField] private bool randomizeHorizontal = true;

    [Tooltip("Guaranteed minimum horizontal spacing between neighbors (world units).")]
    [SerializeField, Min(0f)] private float minHorizontalSpacing = 0.5f;

    [Tooltip("0 = no jitter, 1 = maximum allowable jitter while preserving min spacing.")]
    [SerializeField, Range(0f, 1f)] private float jitterStrength = 1f;
    
    [Tooltip("Delay before playing stomp sound upon spawning each leg.")]
    [SerializeField, Min(0f)] private float StompSoundDelay = 0.5f;

    private float leftEdge;
    private float rightEdge;
    private int activeLegs = 0;
    private Coroutine burstRoutine;

    private void Awake()
    {
        if (spawnArea == null)
        {
            Debug.LogError("[OscillatingLegBurstSpawner2D] Missing spawnArea reference.");
            enabled = false;
            return;
        }
        if (legPrefab == null)
        {
            Debug.LogError("[OscillatingLegBurstSpawner2D] Missing oscillatingLegPrefab reference.");
            enabled = false;
            return;
        }
        
        UpdateHorizontalBounds();

        Debug.Log($"[OscillatingLegBurstSpawner2D] Ready. Area X: {leftEdge:0.###} → {rightEdge:0.###}");
    }


    // Fire a burst of OscillatingLegs evenly spaced left→right across the collider.
    // count<=0 uses defaultBurstCount; betweenDelay<0 uses defaultBetweenSpawnDelay.
    public void TriggerBurst(int count = -1, float betweenDelay = -1f)
    {
        int spawnCount = (count > 0) ? count : defaultBurstCount;
        float delay = (betweenDelay >= 0f) ? betweenDelay : defaultBetweenSpawnDelay;
        
        UpdateHorizontalBounds();

        if (burstRoutine != null)
            StopCoroutine(burstRoutine);

        burstRoutine = StartCoroutine(BurstSequence(spawnCount, delay));
    }
    
    
    private void UpdateHorizontalBounds()
    {
        var b = spawnArea.bounds;
        leftEdge  = b.min.x;
        rightEdge = b.max.x;
    }
    

    private IEnumerator BurstSequence(int count, float betweenDelay)
    {
        if (count <= 0) yield break;

        float y = transform.position.y;
        float width = rightEdge - leftEdge;

        for (int i = 0; i < count; i++)
        {
            // Base even spacing (inclusive/exclusive of edges)
            float xBase;
            float step;

            if (count == 1)
            {
                xBase = (leftEdge + rightEdge) * 0.5f; // single: center
                step = width; // unused
            }
            else if (includeEdges)
            {
                step = width / (count - 1);
                xBase = leftEdge + step * i; // first at left, last at right
            }
            else
            {
                step = width / (count + 1);
                xBase = leftEdge + step * (i + 1); // no exact edges
            }

            // Compute max jitter that preserves min spacing:
            // Worst-case neighbor gap after jitter = step - 2*jitterMax >= minHorizontalSpacing
            float jitterMax = Mathf.Max(0f, (step - minHorizontalSpacing) * 0.5f);
            jitterMax *= jitterStrength;

            // Apply jitter, keeping edges from escaping outside
            float x = xBase;
            if (randomizeHorizontal && jitterMax > 0f && count > 1)
            {
                if (includeEdges && i == 0)
                {
                    // leftmost: only jitter inward (to the right)
                    x += Random.Range(0f, jitterMax);
                }
                else if (includeEdges && i == count - 1)
                {
                    // rightmost: only jitter inward (to the left)
                    x += Random.Range(-jitterMax, 0f);
                }
                else
                {
                    // middle ones: symmetric jitter
                    x += Random.Range(-jitterMax, jitterMax);
                }
            }
            
            if (includeEdges)
                x = Mathf.Clamp(x, leftEdge, rightEdge);
            else
                x = Mathf.Clamp(x, leftEdge + step * 0.5f, rightEdge - step * 0.5f);

            SpawnOne(new Vector2(x, y));

            if (betweenDelay > 0f) 
                yield return new WaitForSeconds(betweenDelay);
            else 
                yield return null;
        }

        burstRoutine = null;
    }

    private void SpawnOne(Vector2 position)
    {
        var go = Instantiate(legPrefab, position, Quaternion.identity);
        activeLegs++;

        // Hook the OscillatingLeg's destruction event (mirrors your existing pattern)
        var leg = go.GetComponent<OscillatingLeg>();
        if (leg != null)
        {
            leg.OnDestroyed += HandleLegDestroyed;
        }
        else
        {
            Debug.LogWarning("[OscillatingLegBurstSpawner2D] Spawned prefab missing OscillatingLeg component. Active count may desync.");
        }

        StartCoroutine(PlayStompSoundWithDelay());
    }
    
    private IEnumerator PlayStompSoundWithDelay()
    {
        yield return new WaitForSeconds(StompSoundDelay);
        CoopAttackSoundPlayer.instance.PlaySoundSingle();
    }

    private void HandleLegDestroyed()
    {
        activeLegs = Mathf.Max(0, activeLegs - 1);
    }
}
