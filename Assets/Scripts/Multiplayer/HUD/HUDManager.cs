using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDManager : MonoBehaviour
{
    static public bool HideAll = false;
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

        HideAll = false;
    }

    private void Start()
    {
        GameManager.Instance.OnScoreChange += ChangeCrown;

        if(GameManager.Instance.LevelNumber != 1 && !HideAll)
        {
            ChangeCrown(GameManager.Instance.GetHighestScorePlayer().MainPlayerController);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnScoreChange -= ChangeCrown;
    }

    void Update()
    {
        if (HideAll)
        {
            for (int i = 0; i < hudRefrences.Length; i++)
                hudRefrences[i].gameObject.SetActive(false);
            return;
        }
        for (int i = 0; i < hudRefrences.Length; i++)
        {
            if (i < PlayerEntity.PlayerList.Count && PlayerEntity.PlayerList[i].CombatManager)
            {
                hudRefrences[i].gameObject.SetActive(true);
                hudRefrences[i].SplashArtImage.sprite = splashArts[PlayerEntity.PlayerList[i].CharacterType];
                hudRefrences[i].HpBar.maxValue = PlayerEntity.PlayerList[i].CombatManager.CurrentMaxHealth;
                hudRefrences[i].HpBar.value = PlayerEntity.PlayerList[i].CombatManager.CurrentHealth;
                hudRefrences[i].EnergyBar.maxValue = PlayerEntity.PlayerList[i].CombatManager.MaxStamina;
                hudRefrences[i].EnergyBar.value = PlayerEntity.PlayerList[i].CombatManager.CurrentStamina;
                hudRefrences[i].BlockBar.maxValue = PlayerEntity.PlayerList[i].CombatManager.MaxBlockStamina;
                hudRefrences[i].BlockBar.value = PlayerEntity.PlayerList[i].CombatManager.CurrentBlockStamina;
                hudRefrences[i].ScoreText.text = PlayerEntity.PlayerList[i].MainPlayerController.GetScore().ToString("N0");
                hudRefrences[i].SpecialIcon[PlayerEntity.PlayerList[i].CharacterType].SetActive(true);
                //Debug.Log($"Current block: {PlayerEntity.PlayerList[i].CombatManager.CurrentBlockStamina}");
            }
            else
                hudRefrences[i].gameObject.SetActive(false);
        }
    }

    private void ChangeCrown(MainPlayerController player)
    {
        foreach (var hud in hudRefrences)
            hud.Crown.SetActive(false);

        for (int i = 0; i < PlayerEntity.PlayerList.Count; i++)
        {
            if (PlayerEntity.PlayerList[i].CharacterType == player.CharacterType)
            {
                hudRefrences[i].Crown.SetActive(true);
                break;
            }
        }
    }
}
