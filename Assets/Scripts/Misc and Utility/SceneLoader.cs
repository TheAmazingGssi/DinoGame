using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { MainMenu = 0, CharacterSelect = 1, Level1 = 2, CharacterSelectToUITesting = 3, EdenTestingScene = 4, UITestingScene = 5, Level2 = 6, Level3 = 7};
public class SceneLoader : MonoBehaviour
{
    [SerializeField] Scenes targetScene;

    public void LoadScene(Scenes scene)
    {
        SceneManager.LoadScene((int)scene);
    }
    public void LoadTargetScene()
    {
        LoadScene(targetScene);
    }
}
