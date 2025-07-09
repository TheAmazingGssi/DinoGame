using UnityEngine;

public class ParalaxAnimation : MonoBehaviour
{
    float focalPoint;
    [Header("Refrences")]
    [SerializeField] Camera cam;
    [SerializeField] CameraMovement camMovement;
    [Header("Moving Layers")]
    [SerializeField] GameObject dinomount;
    [SerializeField] ParalaxLayer[] layers;
    [Header("Speed Settings")]
    [SerializeField] float speed;
    [SerializeField] float dinomountStepSpeed;
    [SerializeField] float dinomountStepHeight;
    [SerializeField] float camHeight;
    float dinomountOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camMovement.active = false;
        HUDManager.HideAll = true;
        cam.orthographicSize = camHeight;
        cam.transform.position = new Vector3(0, 0, cam.transform.position.z);
        dinomountOffset = dinomount.transform.position.y;
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
            layerX = layerX % layer.sprite.sprite.bounds.size.x;
            layer.LayerObject.transform.position = new Vector2(layerX, layer.LayerObject.transform.position.y); //x changes, y stays the same
        }

        float dinoY = Mathf.Sin(Time.time * dinomountStepSpeed) * dinomountStepHeight;
        dinoY += dinomountOffset;
        dinomount.transform.position = new Vector2(dinomount.transform.position.x, dinoY) ;
    }
}