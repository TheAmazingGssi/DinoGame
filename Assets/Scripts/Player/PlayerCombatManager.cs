using System;
using UnityEngine;

public class PlayerCombatManager : CombatManager
{
    private float maxStamina;
    private float currentStamina;
    private float staminaRegenRate = 10f; // Moved from MainPlayerController
    private float damageTakenMultiplier = 1f;
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

        args.Damage *= damageTakenMultiplier;
        base.TakeDamage(args);

        if (args.Knockback)
        {
            KnockbackHelper.ApplyKnockback(
                transform,
                args.SourceGO != null ? args.SourceGO.transform : null,
                KnockbackHelper.GetKnockbackForceFromDamage(args.Damage, true)
            );
        }

        animController.TriggerDamaged();
    }

    public void ApplyDamageTakenIncrease(float percentage)
    {
        damageTakenMultiplier += percentage / 100f;
        damageTakenMultiplier = Mathf.Max(0.1f, damageTakenMultiplier);
        Debug.Log($"{gameObject.name} now takes {damageTakenMultiplier * 100}% damage");
    }

    public void ResetDamageTakenMultiplier()
    {
        damageTakenMultiplier = 1;
    }

    public override void RestoreHealthByPercent(float percent)
    {
        base.RestoreHealthByPercent(percent);
        currentStamina = maxStamina;
        Debug.Log($"{gameObject.name} restored {percent * currentMaxHealth} health, now at {currentHealth}/{currentMaxHealth}, stamina: {currentStamina}/{maxStamina}");
    }

    public void IncreaseMaxHealthByPercentage(float percentage)
    {
        float healthBefore = currentHealth;
        float oldMax = currentMaxHealth;
        currentMaxHealth *= 1 + (percentage / 100f);

        float healthRatio = healthBefore / oldMax;
        currentHealth = currentMaxHealth * healthRatio; //Restore current health?

        Debug.Log($"{gameObject.name} max health increased by {percentage}%. New max: {currentMaxHealth}, current: {currentHealth}");
        UpdateHealthBar();
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