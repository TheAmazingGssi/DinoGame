using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { CharacterSelect = 0, GameScene = 1, UITestingScene = 2 };
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
