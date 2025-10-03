using System.Collections;
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
    WinScreen = 6,
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
    
    public void LoadTargetSceneDelayed(float delay)
    {
        StartCoroutine(DelayedLoadScene(delay));
    }


    private IEnumerator DelayedLoadScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(targetScene);
    }
}
