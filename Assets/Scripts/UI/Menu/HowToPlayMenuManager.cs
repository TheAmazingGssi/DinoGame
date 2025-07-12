using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenuManager : MonoBehaviour
{
    [SerializeField] private Button howToPlayButton;

    [Header("Prefab Refrences")]
    [SerializeField] Selectable defaultSelection;

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        defaultSelection.Select();
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);
        howToPlayButton.Select();
    }
}
