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
            if (colorResetCoroutine != null)
            {
                StopCoroutine(colorResetCoroutine);
                colorResetCoroutine = null;
            }
        }
    }

    private void HandleHurt(DamageArgs damageArgs)
    {
        if (manager.AttackManager.IsCurrentlyAttacking)
        {
            manager.AttackManager.InterruptAttack();
        }

        if (!waitingForHurtAnimation)
        {
            isHurt = true;
        }

        manager.SoundPlayer.PlaySound(1, 0.5f);
        manager.SpriteRenderer.color = Color.red;

        if (colorResetCoroutine != null)
        {
            StopCoroutine(colorResetCoroutine);
        }
        colorResetCoroutine = StartCoroutine(BackupColorReset());
    }

    private IEnumerator BackupColorReset()
    {
        yield return new WaitForSeconds(0.5f);
        if (manager.SpriteRenderer.color == Color.red)
        {
            manager.SpriteRenderer.color = Color.white;
            Debug.Log("Backup color reset triggered");
        }
        colorResetCoroutine = null;
    }

    public void OnHurtAnimationComplete()
    {
        waitingForHurtAnimation = false;
        manager.SpriteRenderer.color = Color.white;

        if (colorResetCoroutine != null)
        {
            StopCoroutine(colorResetCoroutine);
            colorResetCoroutine = null;
        }
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
    }
}