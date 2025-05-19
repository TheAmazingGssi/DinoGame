using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] public GameObject[] EmptyPlayers;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < EmptyPlayers.Length; i++)
        {
            if (PlayerEntity.PlayerList.Count > i)
            {
                PlayerEntity.PlayerList[i].SpawnPlayerController(EmptyPlayers[i].transform);
            }
            //emptyPlayers[i].SetActive(false);
        }
    }
}
