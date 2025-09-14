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
    private float maxBlockStamina;
    private float currentBlockStamina;
    private float blockRegenRate;
    private float blockCostPerHit;
    private bool blockLocked; // lock when 0 until >=50% refilled
    
    public float CurrentBlockStamina => currentBlockStamina;
    public float MaxBlockStamina => maxBlockStamina;
    public bool IsBlockLocked => blockLocked;

    //Special Attack Stamina
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;


    public void Initialize(float maxHealth, float maxStamina, MainPlayerController controller, Animator animator,
        float maxBlockStamina = 5f, float blockCostPerHit = 1f, float blockRegenRate = 1f)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;

        this.maxStamina = maxStamina;
        this.currentStamina = maxStamina;

        this.controller = controller;
        this.animator = animator;
        this.animController = GetComponent<AnimationController>();

        // block stamina init
        this.maxBlockStamina = Mathf.Max(0f, maxBlockStamina);
        this.currentBlockStamina = this.maxBlockStamina;
        this.blockCostPerHit = Mathf.Max(0f, blockCostPerHit);
        this.blockRegenRate = Mathf.Max(0f, blockRegenRate);
        this.blockLocked = false;
    }


    public bool HasBlockStamina() => currentBlockStamina > 0.01f && !blockLocked;
    
    private void LockBlockIfEmpty()
    {
        if (currentBlockStamina <= 0.001f)
        {
            currentBlockStamina = 0f;
            blockLocked = true; // guard broken
        }
    }
    
    
    private bool TrySpendBlockForHit()
    {
        if (blockCostPerHit <= 0f) return true;

        if (currentBlockStamina >= blockCostPerHit)
        {
            currentBlockStamina -= blockCostPerHit;

            if (currentBlockStamina <= 0f)
            {
                LockBlockIfEmpty();
                controller?.ForceStopBlocking(); // drop guard on the exact frame it empties
            }
            return true;
        }
        return false;
    }


    public override void TakeDamage(DamageArgs args)
    {
        if (!MainPlayerController.CanBeDamaged) return;
        
        if (controller != null && controller.isBlocking && HasBlockStamina())
        {
            if (TrySpendBlockForHit())
            {
                // fully blocked: no damage/knockback
                return;
            }
            else
            {
                // no stamina or locked → stop blocking so damage applies
                controller.ForceStopBlocking();
            }
        }
        
        args.Damage *= damageTakenMultiplier;
        base.TakeDamage(args);

        if (args.Knockback)
        {
            KnockbackHelper.ApplyKnockback
            (
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

    //todo: new block system
    public void RegenerateBlockStamina(float dt, bool isBlocking)
    {
        if (!isBlocking && maxBlockStamina > 0f)
        {
            currentBlockStamina = Mathf.Min(maxBlockStamina, currentBlockStamina + blockRegenRate * dt);

            // auto-unlock when we reach 50%
            if (blockLocked && currentBlockStamina >= 0.5f * maxBlockStamina)
                blockLocked = false;
        }
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