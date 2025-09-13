using UnityEngine;

public class PositioningByFacingDirection : MonoBehaviour
{
    [SerializeField] private Transform objTransform;
    [SerializeField] private SpriteRenderer objRenderer;
    
    [SerializeField] private MainPlayerController playerController;
    
    [SerializeField] private Transform posRight;
    [SerializeField] private Transform posLeft;
    
    // Update is called once per frame
    void Update()
    {
        objTransform.position = playerController.IsFacingRight ? posRight.position : posLeft.position;
        objTransform.rotation = playerController.IsFacingRight ? posRight.rotation : posLeft.rotation;
    }
}
