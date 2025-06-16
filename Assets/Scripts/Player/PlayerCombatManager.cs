using UnityEngine;

/*public class PlayerCombatManager : CombatManager
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

public class PlayerCombatManager : CombatManager
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Fallen = Animator.StringToHash("Fallen");

    [SerializeField] private MainPlayerController playerController;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMesh damageNumberPrefab;

    public void Initialize(float maxHealth, MainPlayerController controller, Animator playerAnimator)
    {
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth;
        playerController = controller;
        animator = playerAnimator;
        UpdateHealthBar();
    }

    public override void TakeDamage(DamageArgs damageArgs)
    {
        if (currentHealth <= 0 || !MainPlayerController.CanBeDamaged) return;
        base.TakeDamage(damageArgs);
        Debug.Log("Player took: " + damageArgs);
        if (damageNumberPrefab)
        {
            TextMesh damagePrefabClone = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity, transform);
            damagePrefabClone.text = damageArgs.Damage.ToString();
        }
        if (animator != null)
        {
            animator.SetTrigger(Hurt);
        }
        if (currentHealth <= 0)
        {
            animator?.SetTrigger(Fallen);
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
        animator?.ResetTrigger(Fallen);
        Debug.Log($"{playerController.name} revived with {currentHealth} health");
    }
}
