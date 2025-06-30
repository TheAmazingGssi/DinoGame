using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private HUDRefrences[] hudRefrences;
    [SerializeField] private Sprite TerrySplash;
    [SerializeField] private Sprite SpencerSplash;
    [SerializeField] private Sprite ParisSplash;
    [SerializeField] private Sprite AndrewSplash;
    Dictionary<CharacterType, Sprite> splashArts;

    private void Awake()
    {
        splashArts = new Dictionary<CharacterType, Sprite>();
        splashArts.Add(CharacterType.Triceratops, TerrySplash);
        splashArts.Add(CharacterType.Spinosaurus, SpencerSplash);
        splashArts.Add(CharacterType.Parasaurolophus, ParisSplash);
        splashArts.Add(CharacterType.Therizinosaurus, AndrewSplash);
    }

    void Update()
    {
        for (int i = 0; i < hudRefrences.Length; i++)
        {
            if (i < PlayerEntity.PlayerList.Count)
            {
                hudRefrences[i].gameObject.SetActive(true);
                hudRefrences[i].SplashArtImage.sprite = splashArts[PlayerEntity.PlayerList[i].CharacterType];
                hudRefrences[i].HpBar.maxValue = PlayerEntity.PlayerList[i].CombatManager.MaxHealth;
                hudRefrences[i].HpBar.value = PlayerEntity.PlayerList[i].CombatManager.CurrentHealth;
                hudRefrences[i].EnergyBar.maxValue = PlayerEntity.PlayerList[i].CombatManager.MaxStamina;
                hudRefrences[i].EnergyBar.value = PlayerEntity.PlayerList[i].CombatManager.CurrentStamina;
                //hudRefrences[i].ScoreText.text = PlayerEntity.PlayerList[i].CombatManager.score;
            }
            else
                hudRefrences[i].gameObject.SetActive(false);
        }
    }
}
