using UnityEngine;

public class EnemyCombatManager : CombatManager
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");

    [SerializeField] private EnemyManager manager;
    [SerializeField] private TextMesh damageNumberPrefab;

    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }


    public override void TakeDamage(DamageArgs damageArgs)
    {
        base.TakeDamage(damageArgs);
        if (damageNumberPrefab)
        {
            SpawnDamageText(damageArgs);
            manager.Animator.SetTrigger(Hurt);
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