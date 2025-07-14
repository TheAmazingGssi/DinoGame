using UnityEngine;

public class PlayerHurtBox : MonoBehaviour
{
    private PlayerCombatManager combatManager;

    private void Awake()
    {
        combatManager = GetComponentInParent<PlayerCombatManager>();
        if (combatManager == null)
            Debug.LogError("PlayerHurtBox requires a PlayerCombatManager on the parent GameObject!");
    }
}