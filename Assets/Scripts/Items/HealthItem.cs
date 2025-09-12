using UnityEngine;

public class HealthItem : MonoBehaviour
{
    private const string PLAYER = "Player";

    [SerializeField] private float hpPercent = 20f;

    private PlayerCombatManager combatManager;
    private AnimationController animController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
      //  Debug.Log("collision in hp");
        if (collision.CompareTag(PLAYER))
        {
            Debug.Log("collision in hp is player");

            combatManager = collision.GetComponentInParent<PlayerCombatManager>();
            animController = collision.GetComponentInParent<AnimationController>();
            
            
            if (combatManager == null)
            {
                Debug.LogError("PlayerCombatManager not found on player!");
                return;
            }
            
            if (combatManager.CurrentHealth != combatManager.CurrentMaxHealth)
            {
                Debug.Log("collision in hp and combat manager found");

                combatManager.RestoreHealthByPercent(hpPercent);
                animController?.HealBurst();
                Destroy(gameObject);
            }
        }
    }
}
