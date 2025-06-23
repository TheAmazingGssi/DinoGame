using UnityEngine;

public class IllusionOfDepth : MonoBehaviour
{
    const int MULTIPLIER = 100;
    const float FACE = 0.5f;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer faceSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sprite.sortingOrder = -(int)(transform.position.y * 100);
        if(faceSprite)
            faceSprite.sortingOrder = -(int)((transform.position.y + FACE) * 100);
    }
}
