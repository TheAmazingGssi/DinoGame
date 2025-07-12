using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenuManager : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button howToPlayButton;

    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI GameText;

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

    public void Continue()
    {
        controlsText.gameObject.SetActive(false);
        GameText.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        backButton.Select();
    }
    public void Back()
    {
        GameText.gameObject.SetActive(false);
        controlsText.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
        continueButton.Select();
    }
}
