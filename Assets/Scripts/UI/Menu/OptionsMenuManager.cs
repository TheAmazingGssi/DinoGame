using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenuManager : MonoBehaviour
{
    private const string MASTER_VOL = "MasterVol";
    private const string MUSIC_VOL = "MusicVol";
    private const string SFX_VOL = "SFXVol";

    [SerializeField] private Slider masterVol, musicVol, SFXVol;
    [SerializeField] private AudioMixer audioMixer;

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

    public void ChangeMasterVolume()
    {
        audioMixer.SetFloat(MASTER_VOL, masterVol.value);
    }

    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat(MUSIC_VOL, musicVol.value);
    }
    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat(SFX_VOL, SFXVol.value);
    }
}
