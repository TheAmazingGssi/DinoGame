using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    
    public void KillGame()
    {
        Application.Quit();
    }
    public void MoveToCharacterSelect()
    {
        //foreach (PlayerEntity player in PlayerEntity.PlayerList)
        //    player.DestroyMe();

        sceneLoader.LoadScene(Scenes.CharacterSelect);
    }
    public void MoveToMainMenu()
    {
        sceneLoader.LoadScene(Scenes.MainMenu);
    }
}
