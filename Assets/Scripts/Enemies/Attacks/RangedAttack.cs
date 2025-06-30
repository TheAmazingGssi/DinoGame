using System.Collections;
using UnityEngine;

public class RangedAttack : EnemyAttack
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float damageDelay = 0.1f;

    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);

    protected override void ApplyDamage()
    {
        StartCoroutine(FireProjectileAfterDelay());
        base.ApplyDamage();
    }

    private IEnumerator FireProjectileAfterDelay()
    {
        yield return new WaitForSeconds(damageDelay);

        if (projectilePrefab == null) yield break;

        GameObject projInstance = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile projScript = projInstance.GetComponent<EnemyProjectile>();

        if (projScript != null)
        {
            projScript.speed = projectileSpeed;
            projScript.manager = manager;
        }

        Debug.Log("Ranged attack launched a projectile.");
    }
}
