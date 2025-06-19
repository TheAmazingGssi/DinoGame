using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { MainMenu = 0, CharacterSelect = 1, Level1 = 3, CharacterSelectToUITesting = 4, EdenTestingScene = 5, UITestingScene = 6 };
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
