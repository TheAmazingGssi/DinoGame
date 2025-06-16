using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 minVelocity;
    [Tooltip("Velocity will be multiplied by a value rolled randomally between 1 and this variable")]
    [SerializeField] float randomVelocityMultiplier;
    [SerializeField] float topPadding;
    [SerializeField] Sprite[] sprites;
    
    [Header("Refrences")]
    [SerializeField] Camera cam;
    [SerializeField] SpriteRenderer spriteRenderer;
    

    float topOfCamera;
    float bottomOfCamera;
    float speedVariant = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bottomOfCamera = cam.transform.position.y - cam.orthographicSize;
        transform.Translate(speedVariant * minVelocity * Time.deltaTime);
        if(transform.position.y < bottomOfCamera)
            SpawnAgain();
    }

    void SpawnAgain()
    {
        topOfCamera = cam.transform.position.y + cam.orthographicSize;
        float leftPoint = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        float spawnX = Random.Range(leftPoint, cam.transform.position.x);

        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        speedVariant = Random.Range(1f, randomVelocityMultiplier);

        transform.position = new Vector2(spawnX, topOfCamera + topPadding);

    }
}
