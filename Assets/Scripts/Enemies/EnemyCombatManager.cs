using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int HURT = Animator.StringToHash("Hurt");

    [SerializeField] private EnemyManager manager;

    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnTakeDamage += HandleHurt;
    }
     
    [ContextMenu("Take Damage")]
    public void TakeDamageTest()
    {
        TakeDamage(new DamageArgs(1));
    }
    public override void TakeDamage(DamageArgs damageArgs)
    {
        if (damageArgs.SourceMPC != null)
        {
            PlayerCombatManager playerSource = damageArgs.SourceMPC.GetComponent<PlayerCombatManager>();
            if (playerSource != null)
            {
                manager.AttackManager.OnPlayerDealtDamage(playerSource);
            }
            damageArgs.SourceMPC.AddScore((int)(manager.EnemyData.Score * damageArgs.Damage));
        }

        base.TakeDamage(damageArgs);
    }

    private void HandleHurt(DamageArgs damageArgs)
    {
        if (damageArgs.Knockback)
        {
            manager.AttackManager?.ChangeAttackStatue(false);
            
            KnockbackHelper.ApplyKnockback(
                transform,
                damageArgs.SourceGO != null ? damageArgs.SourceGO.transform : null,
                KnockbackHelper.GetKnockbackForceFromDamage(damageArgs.Damage, true)
            );
        }
        
        if (manager.VFXManager)
            manager.VFXManager.TriggerHurtVfx();
        if (!manager.AttackManager.IsCurrentlyAttacking || manager.EnemyData.IsInterruptible)
            manager.Animator.SetTrigger(HURT);
        manager.SoundPlayer.PlaySound(1, 0.5f);
        //if (manager.EnemyData.IsInterruptible)
            manager.SpriteRenderer.color = Color.red;

        StartCoroutine(AnimationDelay());
    }


    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.13f);
        //if (manager.EnemyData.IsInterruptible)
            manager.SpriteRenderer.color = Color.white;
        manager.Animator.ResetTrigger(HURT);
        if (manager.VFXManager)
            manager.VFXManager.ResetHurtVfx();
    }
    
    public virtual void OnHurtAnimationComplete()
    {
        // Optional: reset hurt state or trigger recovery logic
        Debug.Log($"{gameObject.name} hurt animation complete");
    }

    [ContextMenu("Invoke Death")]
    protected override void HandleDeath()
    {
        base.HandleDeath();
    }
}