//- complex revive prompt system
// using UnityEngine;
// using System.Collections;
//
// public class RevivePrompt : MonoBehaviour
// {
//     [SerializeField] private float pressWindow = 0.5f;
//     [SerializeField] private float revivePromptDuration = 4f;
//
//     private MainPlayerController reviver;
//     private MainPlayerController target;
//     private int pressesRequired = 4;
//     private int currentPresses = 0;
//     private bool isActive = false;
//     private float promptStartTime;
//
//     private AnimationController reviverAnim;
//
//     private void Awake()
//     {
//         // Do nothing in Awake; safety check moved to StartRevive
//     }
//
//     public void StartRevive(MainPlayerController reviverPlayer, MainPlayerController targetPlayer)
//     {
//         reviver = reviverPlayer;
//         target = targetPlayer;
//         promptStartTime = Time.time;
//         currentPresses = 0;
//         isActive = true;
//
//         reviverAnim = reviver.GetComponent<AnimationController>();
//         if (reviverAnim == null)
//         {
//             Debug.LogError("Reviver missing AnimationController!");
//             isActive = false;
//             return;
//         }
//
//         reviverAnim.TriggerRevive();
//         StartCoroutine(ReviveCoroutine());
//     }
//
//     private void Update()
//     {
//         if (!isActive) return;
//
//         if (Input.GetButtonDown("Revive") && (Time.time - promptStartTime <= revivePromptDuration))
//         {
//             currentPresses++;
//
//             if (currentPresses >= pressesRequired)
//             {
//                 CompleteRevive();
//             }
//         }
//     }
//
//     private IEnumerator ReviveCoroutine()
//     {
//         yield return new WaitForSeconds(revivePromptDuration);
//
//         if (isActive && currentPresses < pressesRequired)
//         {
//             FailRevive();
//         }
//     }
//
//     private void CompleteRevive()
//     {
//         isActive = false;
//
//         // Call revive logic on target
//         if (target != null)
//         {
//             var combatManager = target.GetComponent<PlayerCombatManager>();
//             if (combatManager != null)
//                 combatManager.Revive(0.5f);
//
//             target.Revived(); // Assuming this resets animations/states
//         }
//
//         if (reviver != null)
//         {
//             reviver.AddScore(10);
//         }
//
//         Destroy(this);
//     }
//
//     private void FailRevive()
//     {
//         isActive = false;
//         Destroy(this);
//     }
// } 

using UnityEngine;

public class RevivePrompt : MonoBehaviour
{
    public void StartRevive(MainPlayerController reviver, MainPlayerController target)
    {
        // Simulate revive process
        Debug.Log($"{reviver.gameObject.name} is reviving {target.gameObject.name}");
        target.Revived();
        Destroy(this);
    }
}