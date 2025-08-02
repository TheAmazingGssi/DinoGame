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
        if (isHurt)
        {
            manager.Animator.SetTrigger(HURT);
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

        //Debug.Log($"Player dealt {damageArgs.Damage}");
        //Debug.Log($"Enemy took {damageArgs.Damage} from {damageArgs.SourceGO.name}");

        base.TakeDamage(damageArgs);
        
        if(currentHealth >= 0)
        {
            isHurt = false;
        }
    }

    private void HandleHurt(DamageArgs damageArgs)
    {
        if (damageArgs.Knockback)
        {
            manager.Animator.SetTrigger(KNOCKBACK);
        }
        else
        {
            manager.Animator.SetTrigger(HURT);
        }

        manager.SoundPlayer.PlaySound(1, 0.5f);
        manager.SpriteRenderer.color = Color.red;
        StartCoroutine(AnimationDelay());
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.13f);
        manager.SpriteRenderer.color = Color.white;
        manager.Animator.ResetTrigger(HURT);
    }

    protected override void HandleDeath()
    {
        Debug.Log("Enemy died in combat manager");
        base.HandleDeath();
    }
}