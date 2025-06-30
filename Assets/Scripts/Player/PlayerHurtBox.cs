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

    //--------------------------------fix?----------------------------------
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            EnemyAttack enemyAttack = other.GetComponent<EnemyAttack>();
            
            if (enemyAttack != null)
            {
                combatManager.TakeDamage(new DamageArgs { Damage = enemyAttack.Damage, SourceGO = other.gameObject });
                Debug.Log($"Player hit by {other.name} for {enemyAttack.Damage} damage");
            }
        }
    }*/
}