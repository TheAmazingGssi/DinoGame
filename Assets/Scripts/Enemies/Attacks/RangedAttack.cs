using UnityEngine;

public class RangedAttack : EnemyAttack
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private EnemyProjectile projectileScript;
    [SerializeField] private float projectileSpeed;

    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    private void Start()
    {

        projectileScript.speed = projectileSpeed;
        projectileScript.manager = manager;

    }

    protected override void ApplyDamage()
    {
        Debug.Log("Launching projectile");
        Instantiate(projectile, transform.position, Quaternion.identity, transform);
        base.ApplyDamage();
    }
}