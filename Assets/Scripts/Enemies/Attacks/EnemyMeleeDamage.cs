using UnityEngine;

public class EnemyMeleeDamage : MonoBehaviour
{
    private const string PLAYER = "Player";

    [SerializeField] private Collider2D attackCollider;
    public void ApplyDamage(float damage)
    {
        if (attackCollider == null) return;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, attackCollider.bounds.size, 0f);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(PLAYER))
            {
                PlayerCombatManager playerCombat = hitCollider.GetComponent<PlayerCombatManager>();
                if (playerCombat != null)
                {
                    DamageArgs damageArgs = new DamageArgs(damage);

                    playerCombat.TakeDamage(damageArgs);
                    Debug.Log($"Enemy dealt {damage} damage to {hitCollider.name}");
                }
            }
        }
    }
}