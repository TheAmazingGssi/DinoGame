using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerEntityPrefab;
    [SerializeField] private Transform SpawnPoint;

    private static readonly List<MainPlayerController> players = new List<MainPlayerController>();

    public static void RegisterPlayer(MainPlayerController player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

    public static void UnregisterPlayer(MainPlayerController player)
    {
        players.Remove(player);
    }

    public static MainPlayerController FindNearestFallenPlayer(Vector3 position, float range)
    {
        MainPlayerController nearest = null;
        float minDistance = range;

        foreach (var player in players)
        {
            if (player.IsFallen())
            {
                float dist = Vector2.Distance(position, player.transform.position);
                if (dist <= minDistance)
                {
                    nearest = player;
                    minDistance = dist;
                }
            }
        }

        return nearest;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject playerEntityObj = Instantiate(PlayerEntityPrefab, SpawnPoint.position, Quaternion.identity);
        PlayerEntity playerEntity = playerEntityObj.GetComponent<PlayerEntity>();

        // Cache action map
        var playerActions = playerInput.actions.FindActionMap("Player");

        // Wire up input events
        playerActions.FindAction("Move").performed += ctx => playerEntity.InvokeMove(ctx);
        playerActions.FindAction("Move").canceled += ctx => playerEntity.InvokeMove(ctx);
        playerActions.FindAction("Attack").performed += ctx => playerEntity.InvokeAttack(ctx);
        playerActions.FindAction("Special").performed += ctx => playerEntity.InvokeSpecial(ctx);
        playerActions.FindAction("Block").performed += ctx => playerEntity.InvokeBlock(ctx);
        playerActions.FindAction("Revive").performed += ctx => playerEntity.InvokeRevive(ctx);
        playerActions.FindAction("Choose").performed += ctx => playerEntity.InvokeConfirmation(ctx);
        playerActions.FindAction("Pause").performed += ctx => playerEntity.InvokePause(ctx);
        playerActions.FindAction("Emote").performed += ctx => playerEntity.InvokeEmote(ctx);

        playerInput.onDeviceLost += ctx => playerEntity.DeviceDisconnected();
    }
}