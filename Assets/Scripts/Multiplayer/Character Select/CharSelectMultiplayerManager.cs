using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharSelectMultiplayerManager : MonoBehaviour
{
    [SerializeField] CharacterType[] characterList;
    [SerializeField] Sprite[] splashArtArray;
    [SerializeField] string[] namesArray;
    [SerializeField] CharacterSelectorRefrenceHolder[] displayers;
    const string notReadyMessege = "Press X to ready";
    const string readyMessege = "Ready!";
    List<CharacterSelect> playerList = new List<CharacterSelect>();
    [SerializeField] SceneLoader loader;
    [SerializeField] GameObject confirmReadyPanel;
    [SerializeField] float allReadyBuffer = 0.1f;
    float allReadyTime;
    Dictionary<CharacterType, Sprite> splashArt = new Dictionary<CharacterType, Sprite>();
    Dictionary<CharacterType, string> names = new Dictionary<CharacterType, string>();

    bool cancelTrigger = false;
    bool confirmTrigger = false;

    private void Awake()
    {
        for (int i = 0; i < characterList.Length; i++)
        {
            splashArt.Add(characterList[i], splashArtArray[i]);
            names.Add(characterList[i], namesArray[i]);
        }
    }

    private void Update()
    {
        if(confirmReadyPanel.activeSelf)
        {
            if (confirmTrigger && Time.time > allReadyTime)
                GoToNextScene();
            
            if(cancelTrigger)
                confirmReadyPanel.SetActive(false);
        }
        cancelTrigger = false;
        confirmTrigger = false;
    }

    private void UpdateCharacters()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            displayers[i].Image.sprite = splashArt[playerList[i].SelectedCharacter];
            displayers[i].NameText.text = names[playerList[i].SelectedCharacter];
        }
    }
    private void UpdateReady()
    {
        bool allReady = true;
        for(int i = 0; i<playerList.Count; i++)
        {
            if (playerList[i].ready)
                displayers[i].ReadyText.text = readyMessege;
            else
            {
                displayers[i].ReadyText.text = notReadyMessege;
                while (CharacterTaken(playerList[i].SelectedCharacter))
                    playerList[i].SelectedCharacter = GetNextCharacter(playerList[i].SelectedCharacter);
                allReady = false;
            }
        }
        UpdateCharacters();
        
        if (allReady && !confirmReadyPanel.activeSelf)
        {
            confirmReadyPanel.SetActive(true);
            allReadyTime = Time.time + allReadyBuffer;
        }

    }
    
    private bool CharacterTaken(CharacterType character)
    {
        for (int i = 0;i < playerList.Count;i++)
            if (playerList[i].SelectedCharacter == character && playerList[i].ready)
                return true;

        return false;
    }

    public CharacterType GetNextCharacter(CharacterType currentCharacter)
    {
        return ChangeCharacter(currentCharacter, 1);
    }
    public CharacterType GetPreviousCharacter(CharacterType currentCharacter)
    {
        return ChangeCharacter(currentCharacter, -1);
    }

    private CharacterType ChangeCharacter(CharacterType currentCharacter, int change)
    {
        //Find the index of the current character
        int index = 0;
        for (int i = 0; i < characterList.Length; i++)
            if (characterList[i]  == currentCharacter)
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
                if (player.SelectedCharacter == characterList[index] && player.ready)
                    colorIsTaken = true;

            //return the color if its not taken
            if (!colorIsTaken)
                return characterList[index];

        }
        return currentCharacter;
    }
    public void PlayerJoined(PlayerInput playerInput)
    {
        bool player1 = PlayerEntity.PlayerList.Count == 0;

        PlayerEntity player = playerInput.GetComponent<PlayerEntity>();

        CharacterSelect characterSelector = player.SpawnCharacterSelector();

        playerList.Add(characterSelector);
        characterSelector.UpdateColors.AddListener(UpdateCharacters);
        characterSelector.UpdateReady.AddListener(UpdateReady);
        characterSelector.Manager = this;
        displayers[playerList.Count-1].gameObject.SetActive(true);

        player.Cancel.AddListener(OnCancel);

        if (player1)
        {
            player.Confirmation.AddListener(OnConfirm);
        }


        characterSelector.SelectedCharacter = characterList[characterList.Length - 1];
        characterSelector.SelectedCharacter = GetNextCharacter(characterSelector.SelectedCharacter);
        UpdateCharacters();
    }

    private void OnConfirm(InputAction.CallbackContext inputContext)
    {
        confirmTrigger = true;
    }
    private void OnCancel(InputAction.CallbackContext inputContext)
    {
        cancelTrigger = true;
    }

    private void GoToNextScene()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].FinalizeSelection.Invoke();
        }
        loader.LoadTargetScene();
    }
}
