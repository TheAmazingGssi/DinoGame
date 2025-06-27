using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Scene Refrences")]
    [Tooltip("The button which will open this menu")]
    [SerializeField] Button OptionsButton;

    [Header("Prefab Refrences")]
    [SerializeField] Selectable defaultSelection; //the first thing highlighted when the menu opens


    public void OpenMenu()
    {
        gameObject.SetActive(true);
        defaultSelection.Select();
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);
        OptionsButton.Select();
    }
}
