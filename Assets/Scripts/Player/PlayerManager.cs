using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerEntityPrefab;
    [SerializeField] private Transform SpawnPoint;

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject playerEntityObj = Instantiate(PlayerEntityPrefab, SpawnPoint.position, Quaternion.identity);
        PlayerEntity playerEntity = playerEntityObj.GetComponent<PlayerEntity>();

        // Wire up input events
        playerInput.actions.FindActionMap("Player").FindAction("Move").performed += ctx => playerEntity.InvokeMove(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Move").canceled += ctx => playerEntity.InvokeMove(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Attack").performed += ctx => playerEntity.InvokeAttack(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Special").performed += ctx => playerEntity.InvokeSpecial(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Block").performed += ctx => playerEntity.InvokeBlock(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Revive").performed += ctx => playerEntity.InvokeRevive(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Choose").performed += ctx => playerEntity.InvokeConfirmation(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Pause").performed += ctx => playerEntity.InvokePause(ctx);
        playerInput.actions.FindActionMap("Player").FindAction("Emote").performed += ctx => playerEntity.InvokeEmote(ctx);

        
        // Optional: Handle device disconnection
        playerInput.onDeviceLost += ctx => playerEntity.DeviceDisconnected();
    }
}