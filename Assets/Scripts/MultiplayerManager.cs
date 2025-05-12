using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] private GameObject[] emptyPlayersArr;
    Dictionary<int, GameObject> emptyPlayers = new Dictionary<int, GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < emptyPlayersArr.Length; i++)
            emptyPlayers.Add(i, emptyPlayersArr[i]);
    }
    public void SpawnPlayer(PlayerInput playerInput)
    {
        players.Add(playerInput);
        SpawnPlayer(players.Count - 1);
    }
    private void SpawnPlayer(int id)
    {
        //Debug.Log($"Spawning player with {players[id].devices[0]} controller");
        Color character = MultiPlayerInformation.Instance.GetCharacter(players[id].devices[0]);
        players[id].GetComponentInChildren<SpriteRenderer>().color = character;
        players[id].transform.position = emptyPlayers[id].transform.position;
        Destroy(emptyPlayers[id]);
    }
}
