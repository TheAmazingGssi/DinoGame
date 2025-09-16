using System;
using UnityEngine;

public class MockPlayerFloat : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatRotationSpeed = 0.5f;

    private void Start()
    {
        rb.angularVelocity = floatRotationSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.position.x > 0 && other.tag == "SpaceCol")
        {
            rb.linearVelocity = Vector2.left * floatSpeed;
            rb.angularVelocity = -1 * floatRotationSpeed;
        }
        else if (other.transform.position.x < 0 && other.tag == "SpaceCol")
        {
            rb.linearVelocity = Vector2.right * floatSpeed;
            rb.angularVelocity = floatRotationSpeed;
        }
    }
}
