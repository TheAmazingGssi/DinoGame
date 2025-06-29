using System;
using UnityEngine;

public class PDamageArgs
{
    public float Damage { get; set; }
    public GameObject Source { get; set; }
}

public class PlayerCombatManager : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    private MainPlayerController controller;
    private Animator animator;
    private AnimationController animController;
    public event Action<PDamageArgs> OnDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public void Initialize(float maxHealth, MainPlayerController controller, Animator animator)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth; // Initialize runtime health
        this.controller = controller;
        this.animator = animator;
        this.animController = GetComponent<AnimationController>();
    }

    public void TakeDamage(PDamageArgs args)
    {
        if (!MainPlayerController.CanBeDamaged) return;

        currentHealth -= args.Damage;
        Debug.Log($"{gameObject.name} took {args.Damage} damage, health: {currentHealth}");
        animController.TriggerDamaged();

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(args);
        }
    }

    public void RestoreHealthByPercent(float percent)
    {
        float healthToAdd = maxHealth * (percent / 100f);
        currentHealth = Mathf.Min(currentHealth + healthToAdd, maxHealth);
        Debug.Log($"{gameObject.name} restored {healthToAdd} health, now at {currentHealth}/{maxHealth}");
    }
}