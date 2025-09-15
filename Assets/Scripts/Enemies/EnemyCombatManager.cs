using System.Collections;
using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int HURT = Animator.StringToHash("Hurt");

    [SerializeField] private EnemyManager manager;
    
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private Transform damagePopupAnchor;
    [SerializeField] private Vector2 damagePopupJitter = new Vector2(0.25f, 0.15f);
    [SerializeField] private Color damagePopupColor = new Color(0.9960785f, 0.6784314f, 0.1098039f);


    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnTakeDamage += HandleHurt;
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
        int shownDamage = Mathf.RoundToInt(damageArgs.Damage);
        Vector3 spawnPos =
            (
                damagePopupAnchor ? damagePopupAnchor.position : transform.position) +
                new Vector3(Random.Range(-damagePopupJitter.x, damagePopupJitter.x),
                Random.Range(-damagePopupJitter.y, damagePopupJitter.y),
                0f
            );
        
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
        
        if (damagePopupPrefab != null)
        {
            DamagePopup.Spawn(damagePopupPrefab, spawnPos, shownDamage, damagePopupColor);
        }
    }


    private void HandleHurt(DamageArgs damageArgs)
    {
        if (damageArgs.Knockback)
        {
            manager.AttackManager?.ChangeAttackStatue(false);
            
            // use the actual special flag so normals stay little, specials moderate
            KnockbackHelper.ApplyKnockback
            (
                transform,
                damageArgs.SourceGO != null ? damageArgs.SourceGO.transform : null,
                KnockbackHelper.GetKnockbackForceFromDamage(damageArgs.Damage, damageArgs.Knockback)
            );
        }
        
        if(manager.EnemyData.Type != EnemyType.Boss)
            manager.Animator.SetTrigger(HURT);
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
    
    public virtual void OnHurtAnimationComplete()
    {
        Debug.Log($"{gameObject.name} hurt animation complete");
    }

    [ContextMenu("Invoke Death")]
    protected override void HandleDeath()
    {
        base.HandleDeath();
    }
}