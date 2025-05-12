using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPlayerInformation : MonoBehaviour
{
    static public MultiPlayerInformation Instance;

    List<PlayerInformation> players = new List<PlayerInformation>();

    [SerializeField] PlayerInformation[] debugStartingParty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (debugStartingParty.Length > 0)
        {
            for (int i =0; i < debugStartingParty.Length; i++)
                AddPlayerInfo(debugStartingParty[i]);
        }
    }

    public void AddPlayerInfo(PlayerInformation info)
    {
        players.Add(info);
    }
    public void AddPlayerInfo(int index, Color color, InputDevice device)
    {
        AddPlayerInfo(new PlayerInformation(index, color, device.ToString()));
        Debug.Log("Added a player");
    }

    public Color GetCharacter(int index)
    {
        foreach (PlayerInformation info in players)
            if (info.index == index)
                return info.color;

        throw new Exception("No Player with said index");
    }
    public Color GetCharacter(InputDevice device)
    {
        foreach (PlayerInformation info in players)
        {
            Debug.Log("Checking if \"" + info.deviceName + "\" is \"" + device.ToString() + "\"");
            if (info.deviceName == device.ToString())
                return info.color;
        }

        throw new Exception("No Player with said device");
    }
}

[Serializable]
public struct PlayerInformation
{
    public int index;
    public Color color;
    public string deviceName;

    public PlayerInformation(int index, Color color, string device)
    {
        Debug.Log("Saving controller with this name \"" + device + "\"");
        this.index = index;
        this.color = color;
        deviceName = device;
    }
}
