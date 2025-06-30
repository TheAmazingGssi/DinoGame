using System;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    private float maxHealth;
    private float currentHealth;
    private float maxStamina;
    private float currentStamina;
    private float staminaRegenRate = 10f; // Moved from MainPlayerController
    private MainPlayerController controller;
    private Animator animator;
    private AnimationController animController;
    public event Action<DamageArgs> OnDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    public void Initialize(float maxHealth, float maxStamina, MainPlayerController controller, Animator animator)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.maxStamina = maxStamina;
        this.currentStamina = maxStamina;
        this.controller = controller;
        this.animator = animator;
        this.animController = GetComponent<AnimationController>();
    }

    public void TakeDamage(DamageArgs args)
    {
        if (!MainPlayerController.CanBeDamaged) return;

        currentHealth -= args.Damage;
        Debug.Log($"{gameObject.name} took {args.Damage} damage, health: {currentHealth}");
        animController.TriggerDamaged();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke(args);
        }
    }

    public void RestoreHealthByPercent(float percent)
    {
        float healthToAdd = maxHealth * (percent / 100f);
        currentHealth = Mathf.Min(currentHealth + healthToAdd, maxHealth);
        currentStamina = maxStamina; // Reset stamina on revive
        Debug.Log($"{gameObject.name} restored {healthToAdd} health, now at {currentHealth}/{maxHealth}, stamina: {currentStamina}/{maxStamina}");
    }

    public bool DeductStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            Debug.Log($"{gameObject.name} used {amount} stamina, now at {currentStamina}/{maxStamina}");
            return true;
        }
        Debug.Log($"{gameObject.name} insufficient stamina: {currentStamina}/{amount}");
        return false;
    }

    public void RegenerateStamina(float deltaTime)
    {
        currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * deltaTime);
    }
}