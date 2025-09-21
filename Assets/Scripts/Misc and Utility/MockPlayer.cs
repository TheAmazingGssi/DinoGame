using System;
using UnityEngine;

public class MockPlayer : MonoBehaviour
{
    [SerializeField] public EndScreenManager endManager;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float floatRotationSpeed = 3f;
    [SerializeField] private float absFloatDistance = 1f;

    public Vector3 bobbingMotionCenter;

    private void Start()
    {
        if (endManager.StartInLandingPos)
            return;

        if (bobbingMotionCenter == null)
        {
            bobbingMotionCenter = transform.position;
            Debug.Log("bobbingMotionCenter was null, setting to current position");
        }
        
        float direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        rb.angularVelocity = floatRotationSpeed * direction;
        rb.linearVelocity = floatSpeed * new Vector3(0, direction);
    }


    private void FixedUpdate()
    {
        if(transform.position.y >= bobbingMotionCenter.y + absFloatDistance)
            rb.linearVelocity = Vector2.down * floatSpeed;
        else if(transform.position.y <= bobbingMotionCenter.y - absFloatDistance)
            rb.linearVelocity = Vector2.up * floatSpeed;
    }

    // If the player collides with a space collider, reverse their horizontal direction
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SpaceCol")
        {
            if (other.transform.position.x > 0 )
                rb.linearVelocity = Vector2.left * floatSpeed;
            else 
                rb.linearVelocity = Vector2.right * floatSpeed;
        }
    }*/
}
