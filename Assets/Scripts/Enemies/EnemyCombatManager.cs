using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");

    [SerializeField] private EnemyManager manager;
    [SerializeField] private TextMesh damageNumberPrefab;

    [SerializeField] private bool isDead = false;

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
           // isDead = false;
        }
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
        base.TakeDamage(damageArgs);

        if (damageNumberPrefab)
        {
            SpawnDamageText(damageArgs);
            manager.Animator.SetTrigger(Hurt);
        }

        if (damageArgs.Source != null)
        {
            PlayerCombatManager playerSource = damageArgs.Source.GetComponent<PlayerCombatManager>();
            if (playerSource != null)
            {
                manager.OnPlayerDealtDamage(playerSource);
            }
        }
    }

    private void SpawnDamageText(DamageArgs damageArgs)
    {
        TextMesh damagePrefabClone = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity, transform);
        damagePrefabClone.text = damageArgs.Damage.ToString();
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
    }
}