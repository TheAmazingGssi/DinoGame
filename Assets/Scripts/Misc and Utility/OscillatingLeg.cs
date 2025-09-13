using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class OscillatingLeg : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;

    [SerializeField] private float lowerLimitY;
    [SerializeField] private float upperLimitY;

    public event Action OnDestroyed;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (rb) rb.linearVelocity = Vector2.down * speed;
    }

    private void FixedUpdate()
    {
        if (!rb) return;

        float posY = rb.position.y;
        Vector2 vel = rb.linearVelocity;

        if (posY <= lowerLimitY && vel.y < 0f)
        {
            vel.y = Mathf.Abs(speed);        // go up
            rb.linearVelocity = vel;
        }
        else if (posY >= upperLimitY && vel.y > 0f)
        {
            Destroy(gameObject);             // ensure this component's OnDestroy runs
        }
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
        OnDestroyed = null; // defensive cleanup
    }
}