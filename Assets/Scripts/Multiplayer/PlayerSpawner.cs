using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] public Transform[] playerSpawns;
    [SerializeField] private bool SpawnOnStart = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SpawnOnStart)
            Spawn();
    }

    public void Spawn()
    {
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            if (PlayerEntity.PlayerList.Count > i)
            {
                PlayerEntity.PlayerList[i].SpawnPlayerController(playerSpawns[i]);
            }
        }
    }
}
