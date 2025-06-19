using UnityEngine;

public class UIControllerSpawner : MonoBehaviour
{
    [SerializeField] MultiplayerButton defaultOption;

    private void Update()
    {
        //for debugging
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnControllers();
    }

    public void SpawnControllers()
    {
        for (int i = 0; i < PlayerEntity.PlayerList.Count; i++)
        {
            PlayerEntity.PlayerList[i].SpawnUIController(defaultOption);
        }
    }
    public void DebugLog(string msg)
    {
        Debug.Log(msg);
    }
}
