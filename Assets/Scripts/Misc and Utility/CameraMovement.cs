using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool active = true;
    [Header("Zoom Settings")]
    [Tooltip("Empty space between the players and the edge of the camera if the camera is zooming")]
    [SerializeField] float zoomPadding;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    [Header("Settings")]
    [Tooltip("In game this will be changed by a script that reveals more of the stage as the player beats enemy waves. You set the starting values")]
    public float FurthestRightPoint; 
    [Tooltip("In game this will be changed by a script that blocks the way back as the player beats enemy waves. You set the starting values")]
    public float FurthestLeftPoint;
    [SerializeField] float bottomOfStage;
    
    [Header("Component Refrences")]
    [SerializeField] Transform ColliderRight;
    [SerializeField] Transform ColliderLeft;
    [SerializeField] Transform ColliderTop;
    [SerializeField] Transform ColliderBot;
    [SerializeField] Camera mainCamera;
    
    float cameraHalfWidth;
    float cameraHalfHeight;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        active = true;
        UpdateHeightWidth();
        MoveColliders();
    }

    // Update is called once per frame
    void Update()
    {
        if(!active)
            return;

        if (PlayerEntity.PlayerList.Count == 0)
            return;

        ZoomToAverage();
        UpdateHeightWidth();
        MoveToAverage();
        MoveColliders();
    }
    
    private void MoveToAverage()
    {
        //calculate the average
        float cameraX = 0;
        int counter = 0;
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            if (!player.GetCharactersTransform)
                break;
            cameraX += player.GetCharactersTransform.position.x;
            counter++; //making sure we dont jitter if we skipped one transform
        }
        if(counter != 0)
            cameraX /= counter;

        //clamp to not go over where the camera cant go yet
        cameraX = Mathf.Clamp(cameraX, FurthestLeftPoint, FurthestRightPoint);

        float cameraY = bottomOfStage + cameraHalfHeight;

        transform.position = new Vector3(cameraX, cameraY, transform.position.z);
    }
    
    private void MoveColliders()
    {
        ColliderRight.position = new Vector2(transform.position.x + cameraHalfWidth, transform.position.y);
        ColliderLeft.position = new Vector2(transform.position.x - cameraHalfWidth, transform.position.y);
        ColliderTop.position = new Vector2(transform.position.x, transform.position.y + cameraHalfHeight);
        ColliderBot.position = new Vector2(transform.position.x, transform.position.y - cameraHalfHeight);
    }
    
    private void UpdateHeightWidth()
    {
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        cameraHalfHeight = mainCamera.orthographicSize;
    }
    
    private void ZoomToAverage()
    {
        float minX = FurthestRightPoint;
        float maxX = FurthestLeftPoint;
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            if (!player.GetCharactersTransform)
                break;
            if (player.GetCharactersTransform.position.x > maxX)
                maxX = player.GetCharactersTransform.position.x;
            if (player.GetCharactersTransform.position.x < minX)
                minX = player.GetCharactersTransform.position.x;
        }

        float newZoom = (maxX - minX + (zoomPadding * 2)) * Screen.height/ Screen.width * 0.5f;
        //Mathf.Clamp(newZoom, minZoom, maxZoom);
        if (newZoom < minZoom)
            newZoom = minZoom;
        else if (newZoom > maxZoom)
            newZoom = maxZoom;

        mainCamera.orthographicSize = newZoom;
    }
}
