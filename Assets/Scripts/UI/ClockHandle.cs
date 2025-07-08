using UnityEngine;

public class ClockHandle : MonoBehaviour
{
    [SerializeField] RectTransform handle;

    public void SetHandle(float totalTime, float currentTime)
    {
        handle.rotation = Quaternion.Euler(new Vector3(0, 0, (currentTime * -360)/ totalTime));
    }
}
