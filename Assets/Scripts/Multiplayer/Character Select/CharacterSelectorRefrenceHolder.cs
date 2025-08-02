using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectorRefrenceHolder : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI readyText;
    [SerializeField] TextMeshProUGUI nameText;
    public Image Image { get => image; }
    public TextMeshProUGUI ReadyText { get => readyText; }
    public TextMeshProUGUI NameText { get => nameText; }
}
