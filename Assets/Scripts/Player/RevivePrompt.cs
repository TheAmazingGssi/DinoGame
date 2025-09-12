using UnityEngine;

[RequireComponent(typeof(ReviveUI))]
public class ReviveMiniGame : MonoBehaviour
{
    [Header("Revive (Hold) Rules")]
    [SerializeField] private float requiredHoldSeconds = 4f; // todo: modular hold time
    [SerializeField] private float interactionRadius   = 2f; // distance to keep holding

    [Header("On Release Behavior")]
    [SerializeField] private bool  resetOnRelease = true;     // true = snap to 0 on release
    [SerializeField] private float decayPerSecond = 0f;       // used only if resetOnRelease == false (0 = pause)

    private ReviveUI ui;
    private MainPlayerController target;   // this fallen player
    private MainPlayerController reviver;  // who is reviving

    private bool  sessionActive;           // an attempt is active
    private bool  isHolding;               // current input state
    private float heldSeconds;             // progress timer (0..requiredHoldSeconds)

    public bool IsActive => sessionActive;

    private void Awake()
    {
        ui     = GetComponent<ReviveUI>();
        target = GetComponent<MainPlayerController>();
    }

    private void LateUpdate()
    {
        // Keep UI visible while downed, hide only when revived
        if (target != null && !target.IsFallen())
        {
            // Treat as success path for hiding purposes
            if (sessionActive) End(true);
            else ui.Hide();
            return;
        }

        if (!sessionActive) return;

        // Out-of-range cancels HOLD but keeps UI
        if (reviver == null ||
            Vector2.Distance(reviver.transform.position, target.transform.position) > interactionRadius)
        {
            StopHoldInternal();
            return;
        }

        // Progress while holding
        if (isHolding)
        {
            heldSeconds += Time.deltaTime;
        }
        else
        {
            if (resetOnRelease)
            {
                heldSeconds = 0f;
            }
            else if (decayPerSecond > 0f)
            {
                heldSeconds = Mathf.Max(0f, heldSeconds - decayPerSecond * Time.deltaTime);
            }
            // else: pause (no change)
        }

        // Clamp + UI percent
        heldSeconds = Mathf.Clamp(heldSeconds, 0f, requiredHoldSeconds);
        int percent = requiredHoldSeconds <= 0f
            ? 100
            : Mathf.RoundToInt((heldSeconds / requiredHoldSeconds) * 100f);
        ui.SetPercent(percent);

        // Success
        if (percent >= 100)
        {
            target.Revived();
            End(true);
        }
    }

    //todo: revive-hold begin
    public void BeginHold(MainPlayerController reviverPlayer)
    {
        if (!target || !target.IsFallen()) return;

        // Start session if not active; otherwise just (re)enable hold
        if (!sessionActive)
        {
            reviver      = reviverPlayer;
            sessionActive = true;
            heldSeconds   = 0f;
            ui.Show();
            ui.SetPercent(0);
        }

        // If a different reviver arrives, you can choose to transfer ownership:
        if (reviver != reviverPlayer)
            reviver = reviverPlayer;

        isHolding = true;
    }

    //todo: revive-hold stop
    public void StopHold(MainPlayerController who)
    {
        if (who == reviver)
            StopHoldInternal();
    }

    private void StopHoldInternal()
    {
        isHolding = false;
        // Keep sessionActive true so the UI persists and we can resume if they hold again.
        // Progress reset/decay handled in LateUpdate based on flags.
    }

    private void End(bool success)
    {
        sessionActive = false;
        isHolding     = false;
        heldSeconds   = 0f;
        reviver       = null;

        if (success)
        {
            ui.Hide();          // hide only on successful revive / no longer fallen
        }
        else
        {
            ui.SetPercent(0);   // keep visible at 0%
        }
    }

    // Optional helpers if you want to show/hide on downed/revived externally
    public void ShowOnDowned() { ui.Show(); ui.SetPercent(0); }
    public void HideOnRevived() { ui.Hide(); }
    
    public bool CanBegin(MainPlayerController reviverPlayer, float radiusOverride)
    {
        if (!target || !target.IsFallen()) return false;
        float r = radiusOverride > 0f ? radiusOverride : interactionRadius;
        return Vector2.Distance(reviverPlayer.transform.position, target.transform.position) <= r;
    }
}
