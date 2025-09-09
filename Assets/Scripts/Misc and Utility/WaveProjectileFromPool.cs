using System;
using UnityEngine;

public class WaveProjectileFromPool : MonoBehaviour
{
    private Vector3 launchPosition;
    private Vector3 moveDirection;
    private float moveSpeed;
    private float maxDistance;
    private Action<WaveProjectileFromPool> returnToPool;
    [SerializeField] private Animator cachedAnimator;
    
    
    public void Launch(Vector2 direction, float speed, float distance, Action<WaveProjectileFromPool> onReturn)
    {
        launchPosition = transform.position;
        moveDirection = new Vector3(direction.x, direction.y, 0f);
        moveSpeed = speed;
        maxDistance = Mathf.Max(0.01f, distance);
        returnToPool = onReturn;

        // Restart animation from beginning (if present)
        if (cachedAnimator != null)
            cachedAnimator.Play(0, 0, 0f);
    }

    private void Update()
    {
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);

        // return to pool when exceeding distance
        if ((transform.position - launchPosition).sqrMagnitude >= maxDistance * maxDistance)
            returnToPool?.Invoke(this);
    }
}