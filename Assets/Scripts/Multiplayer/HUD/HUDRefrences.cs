using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDRefrences : MonoBehaviour
{
    public Image SplashArtImage; 
 
    public Slider HpBar;
    public Slider EnergyBar;
    public Slider BlockBar;
    public TMP_Text ScoreText;
    public GameObject Crown;

    public GameObject TerrySpecial;
    public GameObject SpencerSpecial;
    public GameObject ParisSpecial;
    public GameObject AndrewSpecial;

    public Dictionary<CharacterType, GameObject> SpecialIcon = new Dictionary<CharacterType, GameObject>();

    private void Awake()
    {
        SpecialIcon.Add(CharacterType.Triceratops, TerrySpecial);
        SpecialIcon.Add(CharacterType.Spinosaurus, SpencerSpecial);
        SpecialIcon.Add(CharacterType.Parasaurolophus, ParisSpecial);
        SpecialIcon.Add(CharacterType.Therizinosaurus, AndrewSpecial);
    }
}
