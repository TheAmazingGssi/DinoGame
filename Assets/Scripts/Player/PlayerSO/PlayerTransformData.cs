using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTransformData", menuName = "Scriptable Objects/PlayerTransformData")]
public class PlayerTransformData : ScriptableObject
{
    public Transform PlayerTransform {  get; set; }
}
