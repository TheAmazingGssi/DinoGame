using UnityEngine;
using UnityEngine.Events;

public struct DamageArgs
{
    public float Damage;
    public MainPlayerController Source; //only for players

    public DamageArgs(float damage, MainPlayerController source = null)
    {
        Damage = damage;
        Source = source;
    }
}

//parent class tp player and enemies combat manager, handles death, health changes and damage taking
public class CombatManager : MonoBehaviour
{
   // [SerializeField] protected UI_ProgressBar healthBar;

    protected float currentHealth = 0;

    public float CurrentHealth
    {
        get => currentHealth;
        set { currentHealth = value; }
    }

    protected float currentMaxHealth = 0;

    public float CurrentMaxHealth => currentMaxHealth;

    public event UnityAction<DamageArgs> OnTakeDamage;

    public event UnityAction<CombatManager> OnDeath;

    private void Start()
    {
        UpdateHealthBar();
    }

    public virtual void TakeDamage(DamageArgs damageArgs)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damageArgs.Damage;
        OnTakeDamage?.Invoke(damageArgs);
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
        UpdateHealthBar();
    }

    public void RestoreHealth(float Health)
    {
        currentHealth = Mathf.Clamp(currentHealth + Health, 0, currentMaxHealth);
        UpdateHealthBar();
    }

    public void RestoreHealthByPercent(float percent)
    {
        Debug.Log($"Restoring: {percent}% health to {gameObject.name}");
        float healthToRestore = percent * currentMaxHealth;
        currentHealth = Mathf.Clamp(currentHealth + healthToRestore, 0, currentMaxHealth);
        UpdateHealthBar();
    }

    protected virtual void HandleDeath()
    {
        OnDeath?.Invoke(this);
    }

    protected void UpdateHealthBar() //no health bar yet
    {
        //if (healthBar)
        //{
        //    healthBar.SetFillAmount(currentHealth, currentMaxHealth);
        //}

    }



}
