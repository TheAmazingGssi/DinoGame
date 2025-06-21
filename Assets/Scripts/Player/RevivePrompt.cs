using UnityEngine;
using System.Collections;

public class RevivePrompt : MonoBehaviour
{
    [SerializeField] private float pressWindow = 0.5f;
    [SerializeField] private float revivePromptDuration = 4f;
    private MainPlayerController reviver;
    private MainPlayerController target;
    private int pressesRequired = 4;
    private int currentPresses = 0;
    private bool isActive = false;
    private float promptStartTime;

    private void Awake()
    {
        if (reviver != null && reviver.GetComponent<AnimationController>() == null)
            Debug.LogError("Reviver missing AnimationController!");
    }

    public void StartRevive(MainPlayerController reviverPlayer, MainPlayerController targetPlayer)
    {
        reviver = reviverPlayer;
        target = targetPlayer;
        isActive = true;
        promptStartTime = Time.time;
        reviver.GetComponent<AnimationController>().TriggerReviveBack();
        StartCoroutine(ReviveCoroutine());
    }

    private void Update()
    {
        if (isActive && Input.GetButtonDown("Revive"))
        {
            if (Time.time - promptStartTime <= revivePromptDuration)
            {
                currentPresses++;
                if (currentPresses >= pressesRequired) CompleteRevive();
            }
        }
    }

    private IEnumerator ReviveCoroutine()
    {
        yield return new WaitForSeconds(revivePromptDuration);
        if (isActive && currentPresses < pressesRequired) FailRevive();
    }

    private void CompleteRevive()
    {
        isActive = false;
        target.GetComponent<PlayerCombatManager>().Revive(0.5f);
        target.Revive();
        reviver.AddScore(10);
        Destroy(this);
    }

    private void FailRevive()
    {
        isActive = false;
        Destroy(this);
    }
}