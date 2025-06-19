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
        sceneLoader.LoadScene(Scenes.CharacterSelect);
    }
}
