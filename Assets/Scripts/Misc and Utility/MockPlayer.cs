using System;
using UnityEngine;

public class MockPlayer : MonoBehaviour
{
    [SerializeField] public EndScreenManager endManager;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float floatRotationSpeed = 3f;

    private void Start()
    {
        if (endManager.StartInLandingPos)
            return;
        
        rb.angularVelocity = floatRotationSpeed;
        rb.linearVelocity = Vector2.right * floatSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SpaceCol")
        {
            if (other.transform.position.x > 0 )
                rb.linearVelocity = Vector2.left * floatSpeed;
            else 
                rb.linearVelocity = Vector2.right * floatSpeed;
        }
    }
}
