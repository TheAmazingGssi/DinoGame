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
    [SerializeField] SoundPlayer soundPlayer; //make soundeffects on changning their volume




    public void OpenMenu()
    {
        //set sliders to their actual positions
        float value = 0;
        if (audioMixer.GetFloat(MASTER_VOL, out value))
            masterVol.value = value;
        if (audioMixer.GetFloat(MUSIC_VOL, out value))
            musicVol.value = value;
        if (audioMixer.GetFloat(SFX_VOL, out value))
            SFXVol.value = value;

        gameObject.SetActive(true);
        defaultSelection.Select();
    }
    public void CloseMenu()
    {
        Debug.Log("should close now");
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
        soundPlayer.PlaySound(0);
    }
}
