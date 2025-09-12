using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    [Header("Child world-space UI (set up in Editor)")]
    [SerializeField] private Canvas reviveCanvas;   // assign child 'ReviveUI' Canvas (World Space)
    [SerializeField] private Slider reviveSlider;   // assign child 'ReviveBar' Slider

    private void Reset()
    {
        if (!reviveCanvas) reviveCanvas = GetComponentInChildren<Canvas>(true);
        if (!reviveSlider) reviveSlider = GetComponentInChildren<Slider>(true);
    }

    private void Awake()
    {
        if (!reviveCanvas || !reviveSlider)
        {
            Debug.LogError("[ReviveUI2D] Please assign the child Canvas and Slider.");
            return;
        }

        if (reviveCanvas.renderMode != RenderMode.WorldSpace)
            reviveCanvas.renderMode = RenderMode.WorldSpace;

        reviveCanvas.worldCamera = Camera.main; // ok in 2D
        reviveCanvas.enabled = false;           // disabled by default (as requested)

        reviveSlider.interactable = false;
        var nav = reviveSlider.navigation; nav.mode = Navigation.Mode.None;
        reviveSlider.navigation = nav;

        // Percent mode 0..100
        reviveSlider.wholeNumbers = true;
        reviveSlider.minValue = 0;
        reviveSlider.maxValue = 100;
        reviveSlider.value    = 0;
    }

    public void Show()
    {
        reviveSlider.value = 0;
        reviveCanvas.enabled = true;
    }

    public void SetPercent(int percent)
    {
        reviveSlider.value = Mathf.Clamp(percent, 0, 100);
    }

    public void Hide()
    {
        reviveCanvas.enabled = false;
    }
}