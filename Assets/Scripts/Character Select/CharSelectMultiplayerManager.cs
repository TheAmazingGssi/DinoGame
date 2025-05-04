using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharSelectMultiplayerManager : MonoBehaviour
{
    [SerializeField] Color[] characterList;
    List<CharacterSelect> playerList = new List<CharacterSelect>();
    
    public Color GetNextColor(Color currentColor)
    {
        //Find the index of the current color
        int index = 0;
        for (int i = 0; i < characterList.Length; i++)
            if (characterList[i]  == currentColor)
                index = i;
        
        for (int i = 0; i < characterList.Length; i++) //looping the array once in case all colors are taken (the person trying to swap color has a color)
        {
            //go to next index
            index++;
            if (index == characterList.Length)
                index = 0;

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
        playerList.Add(playerInput.GetComponent<CharacterSelect>());
    }
}
