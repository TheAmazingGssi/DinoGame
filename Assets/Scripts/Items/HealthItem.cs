using UnityEngine;

public class HealthItem : MonoBehaviour
{
    private const string PLAYER = "Player";

    [SerializeField] private float hpPercent = 20f;

    private PlayerCombatManager combatManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER))
        {
            combatManager = collision.GetComponent<PlayerCombatManager>();
            if (combatManager.CurrentHealth != combatManager.CurrentMaxHealth)
            {
                combatManager.RestoreHealthByPercent(hpPercent);
                Destroy(gameObject);
            }
        }
    }
}
