using UnityEngine;
using UnityEngine.Events;

public class PlayerTutorialProxy : MonoBehaviour
{
    // Use your networking ID if you have one; otherwise assign on spawn.
    public int PlayerId { get; private set; }

    public UnityAction<int> OnAttack;           // arg: PlayerId
    public UnityAction<int> OnBlock;            // arg: PlayerId
    public UnityAction<int, float> OnMoved;     // args: PlayerId, deltaMoveSeconds

    [SerializeField] private float moveThreshold = 0.1f; // input/velocity threshold for "is moving"

    public void Init(int playerId)
    {
        PlayerId = playerId;
    }

    public void ReportAttack() => OnAttack?.Invoke(PlayerId);
    public void ReportBlock()  => OnBlock?.Invoke(PlayerId);

    /// Call every frame with your input magnitude or velocity magnitude
    public void ReportMovementSample(float inputMagnitudeOrSpeed)
    {
        if (inputMagnitudeOrSpeed > moveThreshold)
        {
            OnMoved?.Invoke(PlayerId, Time.deltaTime);
        }
    }
}