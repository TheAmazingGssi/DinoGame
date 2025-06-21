/*using UnityEngine;

public class PlayerCombatManager : CombatManager
{
    public void Initialize(float maxHealth)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public override void TakeDamage(DamageArgs damageArgs) //gets called from the enemy
    {
        base.TakeDamage(damageArgs);
        Debug.Log("Player took Damage");
    }


}*/

using UnityEngine;

public class PlayerCombatTest : CombatManager
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Fallen = Animator.StringToHash("Fallen");
    [SerializeField] private PlayerTransformData playerTransform;


    [SerializeField] private TextMesh damageNumberPrefab;

    public void Initialize(float maxHealth)
    {
        playerTransform.PlayerTransform = transform;
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
       // if (currentHealth <= 0) return;
        base.TakeDamage(damageArgs);
        Debug.Log("Player took: " + damageArgs);
        if (damageNumberPrefab)
        {
            TextMesh damagePrefabClone = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity, transform);
            damagePrefabClone.text = damageArgs.Damage.ToString();
        }
    }

    protected override void HandleDeath()
    {
        base.HandleDeath(); // Invokes OnDeath event
    }

    public void Revive(float healthFraction)
    {
        currentHealth = healthFraction * currentMaxHealth;
        UpdateHealthBar();
    }
}
