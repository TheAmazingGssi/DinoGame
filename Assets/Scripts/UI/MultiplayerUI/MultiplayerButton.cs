using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image terryIndicator;
    [SerializeField] Image spencerIndicator;
    [SerializeField] Image parisIndicator;
    [SerializeField] Image andrewIndicator;
    Dictionary<CharacterType, Image> characterIndicators = new Dictionary<CharacterType, Image>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterIndicators.Add(CharacterType.Triceratops, terryIndicator);
        characterIndicators.Add(CharacterType.Spinosaurus, spencerIndicator);
        characterIndicators.Add(CharacterType.Parasaurolophus, parisIndicator);
        characterIndicators.Add(CharacterType.Therizinosaurus, andrewIndicator);

        foreach(Image img in characterIndicators.Values)
            img.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
