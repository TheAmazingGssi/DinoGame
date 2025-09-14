using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;          
    [SerializeField] private float riseSpeed = 1.5f; 
    [SerializeField] private float stayDuration = 0.35f;
    [SerializeField] private float fadeSpeed = 3f;

    private float timer;
    private Color col = Color.white;

    private void Awake()
    {
        if (text == null) text = GetComponentInChildren<TMP_Text>();
        if (text != null) col = text.color;
    }

    public void Setup(int amount, Color? overrideColor = null)
    {
        if (text == null) return;
        text.text = amount.ToString();
        col = overrideColor ?? text.color;
        col.a = 1f;
        text.color = col;
        timer = 0f;
    }

    private void Update()
    {
        // Rise
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Fade after a short delay
        timer += Time.deltaTime;
        if (timer >= stayDuration && text != null)
        {
            col.a -= fadeSpeed * Time.deltaTime;
            text.color = col;
            if (col.a <= 0f) Destroy(gameObject);
        }
    }

    // Factory
    public static DamagePopup Spawn(DamagePopup prefab, Vector3 worldPos, int amount, Color? color = null)
    {
        var popup = Instantiate(prefab, worldPos, Quaternion.identity);
        popup.Setup(amount, color);
        return popup;
    }
}