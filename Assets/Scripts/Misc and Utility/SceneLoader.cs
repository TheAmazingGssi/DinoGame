using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes 
{  
    MainMenu = 0, 
    CharacterSelect = 1,
    Level1 = 2,
    Level2 = 3,
    Level3 = 4 ,
    LoseScreen = 5,
    WinScreenMoon = 6,
    WinScreenSun = 7,
    WinScreenMars = 8
};
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
