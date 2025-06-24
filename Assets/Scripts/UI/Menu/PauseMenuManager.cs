using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] Selectable defaultSelection;

    public void ToMainMenu()
    {
        sceneLoader.LoadScene(Scenes.MainMenu);
    }
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        defaultSelection.Select();
    }
}
