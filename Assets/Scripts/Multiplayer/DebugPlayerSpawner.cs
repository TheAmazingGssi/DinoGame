using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPlayerSpawner : MonoBehaviour
{
    [SerializeField] PlayerSpawner multiplayerManager;
    [SerializeField] CharacterType[] playerCharacters;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerEntity.PlayerList.Count > 0)
        {
            Destroy(this);
            return;
        }
    }

    public void JoinPlayer(PlayerInput playerInput)
    {
        StartCoroutine(PlayerJoin(playerInput));
        //multiplayerManager.SpawnPlayer(PlayerEntity.PlayerList.Count - 1);
    }
    private IEnumerator PlayerJoin(PlayerInput playerInput)
    {
        yield return 1;

        PlayerEntity entity = playerInput.GetComponent<PlayerEntity>();
        Debug.Log(PlayerEntity.PlayerList.Count);
        entity.CharacterType = playerCharacters[PlayerEntity.PlayerList.Count - 1];
        entity.SpawnPlayerController(multiplayerManager.playerSpawns[PlayerEntity.PlayerList.Count - 1].transform);
    }
}
