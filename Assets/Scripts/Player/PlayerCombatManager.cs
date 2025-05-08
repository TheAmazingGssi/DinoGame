using UnityEngine;

public class PlayerCombatManager : CombatManager
{
    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
        base.TakeDamage(damageArgs);
        Debug.Log("Player took Damage" + damageArgs);
    }
}
