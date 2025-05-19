using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float furthestRightPoint;
    [SerializeField] float furthestLeftPoint;
    [SerializeField] Transform DebugItemRight;
    [SerializeField] Transform DebugItemLeft;
    [SerializeField] Camera mainCamera;
    
    float cameraHalfWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerEntity.PlayerList.Count == 0)
            return;

        //calculate the average
        float cameraX = 0;
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
            cameraX += player.GetCharactersTransform.position.x; //this line throws an exception in the frame a new character is generated. for now it only happens while debugging, and every following frame it works as intended so its fine
        cameraX /= PlayerEntity.PlayerList.Count;

        //clamp to not go over where the camera cant go yet
        cameraX = Mathf.Clamp(cameraX, furthestLeftPoint, furthestRightPoint);

        transform.position = new Vector3 (cameraX, transform.position.y, transform.position.z);
    }
}
