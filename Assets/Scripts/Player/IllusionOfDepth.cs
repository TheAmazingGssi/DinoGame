using UnityEngine;

public class IllusionOfDepth : MonoBehaviour
{
    const int MULTIPLIER = 100;

    [SerializeField] private SpriteRenderer sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sprite.sortingOrder = 0-(int)(transform.position.y * 100);
    }
}
