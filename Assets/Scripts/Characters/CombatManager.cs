using UnityEngine;
using UnityEngine.Events;

public struct DamageArgs
{
    public float Damage;
}
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

    protected virtual void HandleDeath()
    {
        OnDeath?.Invoke(this);
    }

    protected void UpdateHealthBar()
    {
        //if (healthBar)
        //{
        //    healthBar.SetFillAmount(currentHealth, currentMaxHealth);
        //}

    }



}
