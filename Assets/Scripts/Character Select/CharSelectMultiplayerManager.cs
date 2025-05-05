using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharSelectMultiplayerManager : MonoBehaviour
{
    [SerializeField] Color[] characterList;
    [SerializeField] Image[] imageList;
    [SerializeField] TextMeshProUGUI[] textArray;
    [SerializeField] GameObject[] displayers;
    const string notReadyMessege = "Press X to ready";
    const string readyMessege = "Ready!";
    List<CharacterSelect> playerList = new List<CharacterSelect>();

    private void UpdateColors()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            imageList[i].color = playerList[i].Color;
        }
    }
    private void UpdateReady()
    {
        bool allReady = true;
        for(int i = 0; i<playerList.Count; i++)
        {
            if (playerList[i].ready)
                textArray[i].text = readyMessege;
            else
            {
                textArray[i].text = notReadyMessege;
                allReady = false;
            }
        }
        if (allReady)
        {
            GoToNextScene();
        }
    }
    
    public Color GetNextColor(Color currentColor)
    {
        return ChangeColor(currentColor, 1);
    }
    public Color GetPreviousColor(Color currentColor)
    {
        return ChangeColor(currentColor, -1);
    }

    private Color ChangeColor(Color currentColor, int change)
    {
        //Find the index of the current color
        int index = 0;
        for (int i = 0; i < characterList.Length; i++)
            if (characterList[i]  == currentColor)
                index = i;
        
        for (int i = 0; i < characterList.Length; i++) //looping the array once in case all colors are taken (the person trying to swap color has a color)
        {
            //go to next index
            index += change;
            if (index >= characterList.Length)
                index = 0;
            else if (index < 0)
                index = characterList.Length - 1;

            //check if the color is taken by anyone else
            bool colorIsTaken = false;
            foreach (CharacterSelect player in playerList)
                if (player.Color == characterList[index])
                    colorIsTaken = true;

            //return the color if its not taken
            if (!colorIsTaken)
                return characterList[index];

        }
        return currentColor;
    }
    public void PlayerJoined(PlayerInput playerInput)
    {
        CharacterSelect newPlayer = playerInput.GetComponent<CharacterSelect>();
        playerList.Add(newPlayer);
        newPlayer.UpdateColors.AddListener(UpdateColors);
        newPlayer.UpdateReady.AddListener(UpdateReady);
        newPlayer.Manager = this;
        displayers[playerList.Count-1].SetActive(true);
        Debug.Log("Added a player");
    }

    private void GoToNextScene()
    {
        Debug.Log("Game Starts");
    }
}
