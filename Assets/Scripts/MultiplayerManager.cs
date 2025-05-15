using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] emptyPlayers;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < emptyPlayers.Length; i++)
        {
            if (PlayerEntity.PlayerList.Count > i)
            {
                PlayerEntity.PlayerList[i].SpawnPlayerController(emptyPlayers[i].transform);
            }
            Destroy(emptyPlayers[i]);
        }
    }
}
