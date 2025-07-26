using UnityEngine;
using System.Collections;

public class MeleeAttack : EnemyAttack
{
    [SerializeField] private float damageDelay = 0.2f;

    protected override bool IsPlayerInRange => IsTargetInRange(AttackRange);
    protected override EnemyAttackType type => EnemyAttackType.Melee;

    protected override void ApplyDamage()
    {
        StartCoroutine(ApplyDamageAfterDelay());
        base.ApplyDamage();
    }

    private IEnumerator ApplyDamageAfterDelay()
    {
        yield return new WaitForSeconds(damageDelay);

        GameObject activeCollider = manager.AttackManager.IsFacingRight ?
            manager.AttackManager.RightAttackCollider :
            manager.AttackManager.LeftAttackCollider;

        if (activeCollider != null)
        {
            EnemyMeleeDamage meleeDamage = activeCollider.GetComponent<EnemyMeleeDamage>();
            if (meleeDamage != null)
            {
                meleeDamage.ApplyDamage(manager.EnemyData.BaseDamage);
                //Debug.Log($"{gameObject.name} applied damage in {(manager.AttackManager.IsFacingRight ? "right" : "left")} direction");
            }
        }
    }
}