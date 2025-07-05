using UnityEngine;

public class ParalaxAnimation : MonoBehaviour
{
    float focalPoint;
    [SerializeField] ParalaxLayer[] layers;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].CameraOffset = focalPoint - layers[i].LayerObject.transform.position.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        focalPoint += speed * Time.deltaTime;

        foreach (ParalaxLayer layer in layers)
        {
            float layerX = (focalPoint * layer.Multiplier) - layer.CameraOffset;
            //layerX = layerX % layer.sprite.sprite;
            layer.LayerObject.transform.position = new Vector2(layerX, layer.LayerObject.transform.position.y); //x changes, y stays the same
        }
    }
}