using System.Collections;
using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int HURT = Animator.StringToHash("Hurt");
    private static readonly int KNOCKBACK = Animator.StringToHash("Knockback");

    [SerializeField] private EnemyManager manager;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isHurt = false;
    [HideInInspector] public bool IsKnockbacked = false;

    // Track if we're waiting for animation events
    private bool waitingForHurtAnimation = false;
    private Coroutine colorResetCoroutine;

    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead)
        {
            HandleDeath();
            isDead = false;
        }

        if (isHurt && !waitingForHurtAnimation)
        {
            manager.Animator.SetTrigger(HURT);
            waitingForHurtAnimation = true;
            isHurt = false;
        }
    }

    private void Start()
    {
        OnTakeDamage += HandleHurt;
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
            damageArgs.SourceMPC.AddScore(manager.EnemyData.Score);
        }

        base.TakeDamage(damageArgs);

        if (currentHealth <= 0)
        {
            isHurt = false;
            waitingForHurtAnimation = false;
            // Stop any color reset coroutine on death
            if (colorResetCoroutine != null)
            {
                StopCoroutine(colorResetCoroutine);
                colorResetCoroutine = null;
            }
        }
    }

    private void HandleHurt(DamageArgs damageArgs)
    {
        // Interrupt any ongoing attack
        if (manager.AttackManager.IsCurrentlyAttacking)
        {
            manager.AttackManager.InterruptAttack();
        }

        // Only use hurt animation - ignore knockback completely
        if (!waitingForHurtAnimation)
        {
            isHurt = true; // This will be processed in Update()
        }

        manager.SoundPlayer.PlaySound(1, 0.5f);
        manager.SpriteRenderer.color = Color.red;

        // Start backup color reset coroutine in case animation event fails
        if (colorResetCoroutine != null)
        {
            StopCoroutine(colorResetCoroutine);
        }
        colorResetCoroutine = StartCoroutine(BackupColorReset());
    }

    private IEnumerator BackupColorReset()
    {
        yield return new WaitForSeconds(0.5f); // Wait longer than animation should take
        if (manager.SpriteRenderer.color == Color.red)
        {
            manager.SpriteRenderer.color = Color.white;
            Debug.Log("Backup color reset triggered");
        }
        colorResetCoroutine = null;
    }

    // Called by AnimationManager when hurt animation ends
    public void OnHurtAnimationComplete()
    {
        waitingForHurtAnimation = false;
        manager.SpriteRenderer.color = Color.white;

        // Cancel backup coroutine since animation event worked
        if (colorResetCoroutine != null)
        {
            StopCoroutine(colorResetCoroutine);
            colorResetCoroutine = null;
        }

        Debug.Log("Hurt animation completed - color reset via animation event");
    }

    protected override void HandleDeath()
    {
        Debug.Log("Enemy died in combat manager");
        base.HandleDeath();
    }
}