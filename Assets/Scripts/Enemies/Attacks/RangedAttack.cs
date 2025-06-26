using UnityEngine;

public class RangedAttack : EnemyAttack
{
    [SerializeField] private GameObject projectile;
    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        Instantiate(projectile);
    }
}
