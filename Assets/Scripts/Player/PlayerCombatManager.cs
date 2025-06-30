using System;
using UnityEngine;


public class PlayerCombatManager : CombatManager
{
    private MainPlayerController controller;
    private Animator animator;
    private AnimationController animController;

    public void Initialize(float maxHealth, MainPlayerController controller, Animator animator)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth; // Initialize runtime health
        this.controller = controller;
        this.animator = animator;
        this.animController = GetComponent<AnimationController>();
    }

    public override void TakeDamage(DamageArgs args)
    {
        if (!MainPlayerController.CanBeDamaged) return;

        Debug.Log($"{gameObject.name} took {args.Damage} damage, health: {currentHealth}");
        animController.TriggerDamaged();

        base.TakeDamage(args);
    }
}