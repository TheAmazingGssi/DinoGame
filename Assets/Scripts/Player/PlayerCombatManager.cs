using UnityEngine;

public class PlayerCombatManager : CombatManager
{
    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public override void TakeDamage(DamageArgs damageArgs) //gets called from the enemy
    {
        base.TakeDamage(damageArgs);
        Debug.Log("Player took Damage");
    }
    
    
}
