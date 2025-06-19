using UnityEngine;

public class UIControllerSpawner : MonoBehaviour
{
    [SerializeField] MultiplayerButton defaultOption;

    private void Update()
    {
        //for debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space");
            SpawnControllers();

        }
    }

    public void SpawnControllers()
    {
        for (int i = 0; i < PlayerEntity.PlayerList.Count; i++)
        {
            Debug.Log("should spawn ui controller");

            PlayerEntity.PlayerList[i].SpawnUIController(defaultOption);
        }
    }
    public void DebugLog(string msg)
    {
        Debug.Log(msg);
    }
}
