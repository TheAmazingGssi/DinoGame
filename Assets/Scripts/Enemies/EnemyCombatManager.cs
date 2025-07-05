using System.Collections;
using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");

    [SerializeField] private EnemyManager manager;
    [SerializeField] private TextMesh damageNumberPrefab;

    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isHurt = false;
    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
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
            manager.Animator.SetTrigger(Hurt);
        }
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
        base.TakeDamage(damageArgs);
        Debug.Log($"Player dealt" + damageArgs.Damage);

        manager.Animator.SetTrigger(Hurt);
        manager.SoundPlayer.PlaySound(1);

        if (damageNumberPrefab)
        {
            SpawnDamageText(damageArgs);
        }

        if (damageArgs.SourceMPC != null)
        {
            PlayerCombatManager playerSource = damageArgs.SourceMPC.GetComponent<PlayerCombatManager>();
            if (playerSource != null)
            {
                manager.AttackManager.OnPlayerDealtDamage(playerSource);
            }
        }

        damageArgs.SourceMPC.AddScore(manager.EnemyData.Score);

        StartCoroutine(AnimationDelay());
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.13f);
        manager.Animator.ResetTrigger(Hurt);
    }

    private void SpawnDamageText(DamageArgs damageArgs)
    {
        TextMesh damagePrefabClone = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity, transform);
        damagePrefabClone.text = damageArgs.Damage.ToString();
    }

    protected override void HandleDeath()
    {
        Debug.Log("Ded combat");
        base.HandleDeath();
    }
}