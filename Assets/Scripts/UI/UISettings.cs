using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[CreateAssetMenu(fileName = "UISettings", menuName = "Scriptable Objects/UISettings")]
public class UISettings : ScriptableObject
{
    [SerializeField] float deadzone;
    [SerializeField] float inputCooldown;

    private float nextReadyTime = 0;

    public Vector2 CheckSettings(Vector2 input)
    {
        //make sure it isnt a wrong input
        if (input.magnitude < deadzone || input.magnitude == 0) //no drift joystick
        {
            if (Time.time <= nextReadyTime)
                nextReadyTime = Time.time;
            return Vector2.zero;
        }
        //not to swap through all the option in a single frame
        if (Time.time <= nextReadyTime)
            return Vector2.zero;
        nextReadyTime = Time.time + inputCooldown;

        return input;
    }
}
