using UnityEngine;

public class Tumbeweed : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float spinSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpHeight;
    //[SerializeField] Sprite sprite;
    [SerializeField] float padding;
    [SerializeField] float maxHeight;
    [SerializeField] float minHeight;
    
    [Header("Refrences")]
    [SerializeField] SpriteRenderer tumbleweedSprite;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Camera cam;

    bool respawns = true;

    float leftPoint;

    // Update is called once per frame
    void Update()
    {
        float height = Mathf.Sin(Time.time * jumpSpeed) * jumpHeight;
        height = Mathf.Abs(height);
        tumbleweedSprite.transform.localPosition = new Vector2(0, height);
        tumbleweedSprite.transform.eulerAngles = new Vector3(0, 0, tumbleweedSprite.transform.eulerAngles.z + (spinSpeed*Time.deltaTime));
        rb.linearVelocityX = -speed;

        leftPoint = cam.transform.position.x - padding - cam.orthographicSize * cam.aspect;
        if(transform.position.x <= leftPoint)
        {
            SpawnAgain();
        }
    }

    void SpawnAgain()
    {
        if(!respawns)
        {
            gameObject.SetActive(false);
            return;
        }

        float rightPoint = cam.transform.position.x + padding + cam.orthographicSize * cam.aspect;
        float spawnY = Random.Range(minHeight, maxHeight);
        //tumbleweedSprite.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.position = new Vector2(rightPoint, spawnY);
    }

    public void DontRespawn()
    {
        respawns = false;
    }
}
