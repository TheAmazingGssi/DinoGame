using System;
using UnityEngine;

public class PlayerCombatManager : CombatManager
{
    private float maxStamina;
    private float currentStamina;
    private float staminaRegenRate = 10f; // Moved from MainPlayerController
    private MainPlayerController controller;
    private Animator animator;
    private AnimationController animController;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    public void Initialize(float maxHealth, float maxStamina, MainPlayerController controller, Animator animator)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        this.maxStamina = maxStamina;
        this.currentStamina = maxStamina;
        this.controller = controller;
        this.animator = animator;
        this.animController = GetComponent<AnimationController>();
    }

    public override void TakeDamage(DamageArgs args)
    {
        if (!MainPlayerController.CanBeDamaged) return;

        base.TakeDamage(args);
        Debug.Log($"{gameObject.name} took {args.Damage} damage, health: {currentHealth}");
        animController.TriggerDamaged();
    }



    public override void RestoreHealthByPercent(float percent)
    {
        base.RestoreHealthByPercent(percent);
        currentStamina = maxStamina;
        Debug.Log($"{gameObject.name} restored {percent * currentMaxHealth} health, now at {currentHealth}/{currentMaxHealth}, stamina: {currentStamina}/{maxStamina}");
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