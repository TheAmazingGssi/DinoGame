using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] ParalaxLayer[] layers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].CameraOffset = mainCamera.transform.position.x - layers[i].LayerObject.transform.position.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ParalaxLayer layer in layers)
        {
            float layerX = (mainCamera.transform.position.x * layer.Multiplier) - layer.CameraOffset;
            layer.LayerObject.transform.position = new Vector2(layerX, layer.LayerObject.transform.position.y); //x changes, y stays the same
        }
    }
}
[System.Serializable]
struct ParalaxLayer
{
    public GameObject LayerObject;
    public float Multiplier;
    [HideInInspector]public float CameraOffset;
}