using System.Collections;
using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int HURT = Animator.StringToHash("Hurt");
    private static readonly int KNOCKBACK = Animator.StringToHash("Hurt");

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
        if(isDead)
        {
            HandleDeath();
            isDead = false;
        }
        if(isHurt)
        {
            manager.Animator.SetTrigger(HURT);
        }
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
        if(true)
        {
            manager.Animator.SetTrigger(KNOCKBACK);
        }
        else
        {
            manager.Animator.SetTrigger(HURT);
        }

        base.TakeDamage(damageArgs);
     //   Debug.Log($"Player dealt" + damageArgs.Damage);

        manager.SoundPlayer.PlaySound(1);
        manager.SpriteRenderer.color = Color.red;

        if (damageArgs.SourceMPC != null)
        {
            PlayerCombatManager playerSource = damageArgs.SourceMPC.GetComponent<PlayerCombatManager>();
            if (playerSource != null)
            {
                manager.AttackManager.OnPlayerDealtDamage(playerSource);
            }
        }

        damageArgs.SourceMPC.AddScore(manager.EnemyData.Score);

       // StartCoroutine(AnimationDelay());
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.13f);
        manager.SpriteRenderer.color = Color.white;
        manager.Animator.ResetTrigger(HURT);
    }

    protected override void HandleDeath()
    {
        Debug.Log("Ded combat");
        base.HandleDeath();
    }
}