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
        while (PlayerEntity.PlayerList.Count > 0)
            PlayerEntity.PlayerList[0].DestroyMe();

        VoteEffectManager.Instance?.ClearAll();

        sceneLoader.LoadScene(Scenes.CharacterSelect);
    }
    public void MoveToMainMenu()
    {
        sceneLoader.LoadScene(Scenes.MainMenu);
    }
}
