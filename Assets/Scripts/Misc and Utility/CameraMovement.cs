using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("In game this will be changed by a script that reveals more of the stage as the player beats enemy waves")]
    [SerializeField] float furthestRightPoint; 
    [Tooltip("In game this will be changed by a script that blocks the way back as the player beats enemy waves")]
    [SerializeField] float furthestLeftPoint;
    [Tooltip("Empty space between the players and the edge of the camera if the camera is zooming")]
    [SerializeField] float zoomPadding;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    [Header("ComponentRefrences")]
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
        UpdateHeightWidth();
        MoveColliders();
    }

    // Update is called once per frame
    void Update()
    {
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
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
            cameraX += player.GetCharactersTransform.position.x; //this line throws an exception in the frame a new character is generated. for now it only happens while debugging, and every following frame it works as intended so its fine
        cameraX /= PlayerEntity.PlayerList.Count;

        //clamp to not go over where the camera cant go yet
        cameraX = Mathf.Clamp(cameraX, furthestLeftPoint, furthestRightPoint);

        transform.position = new Vector3(cameraX, transform.position.y, transform.position.z);
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
        float minX = furthestRightPoint;
        float maxX = furthestLeftPoint;
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
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
