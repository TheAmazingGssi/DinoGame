using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharSelectMultiplayerManager : MonoBehaviour
{
    [SerializeField] Color[] characterList;
    [SerializeField] CharacterSelectorRefrenceHolder[] displayers;
    const string notReadyMessege = "Press X to ready";
    const string readyMessege = "Ready!";
    List<CharacterSelect> playerList = new List<CharacterSelect>();
    [SerializeField] SceneLoader loader;

    private void UpdateColors()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            displayers[i].Image.color = playerList[i].Color;
        }
    }
    private void UpdateReady()
    {
        bool allReady = true;
        for(int i = 0; i<playerList.Count; i++)
        {
            if (playerList[i].ready)
                displayers[i].Text.text = readyMessege;
            else
            {
                displayers[i].Text.text = notReadyMessege;
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
        PlayerEntity player = playerInput.GetComponent<PlayerEntity>();

        CharacterSelect characterSelector = player.SpawnCharacterSelector().GetComponent<CharacterSelect>();

        playerList.Add(characterSelector);
        characterSelector.UpdateColors.AddListener(UpdateColors);
        characterSelector.UpdateReady.AddListener(UpdateReady);
        characterSelector.Manager = this;
        displayers[playerList.Count-1].gameObject.SetActive(true);
        
    }

    private void GoToNextScene()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            MultiPlayerInformation.Instance.AddPlayerInfo(i, playerList[i].Color, playerList[i].PlayerInput.devices[0]);
            //Debug.Log($"Saving player with {playerList[i].PlayerInput.devices[0]} controller");
        }
        loader.LoadTargetScene();
    }
}
