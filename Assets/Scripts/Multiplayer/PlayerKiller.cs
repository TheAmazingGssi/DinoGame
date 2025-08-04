using UnityEngine;

public class PlayerKiller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            Destroy(player);
        }

        PlayerEntity.PlayerList.Clear();
    }
}
