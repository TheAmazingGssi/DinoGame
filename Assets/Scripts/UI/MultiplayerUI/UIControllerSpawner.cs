using UnityEngine;

public class UIControllerSpawner : MonoBehaviour
{
    public void SpawnControllers(MultiplayerButton defaultOption)
    {
        for (int i = 0; i < PlayerEntity.PlayerList.Count; i++)
            PlayerEntity.PlayerList[i].SpawnUIController(defaultOption);
    }
    public void DebugLog(string msg)
    {
        Debug.Log(msg);
    }
}
