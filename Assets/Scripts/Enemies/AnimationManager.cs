using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimationManager : MonoBehaviour
{
    private static readonly int HURT = Animator.StringToHash("Hurt");
    private static readonly int KNOCKBACK = Animator.StringToHash("Knockback");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int AOEAttack = Animator.StringToHash("AOEAttack");

    [SerializeField] private EnemyManager manager;

    private void Update()
    {
        //if (!manager.CombatManager.IsKnockbacked) manager.Animator.speed = 1;
    }

    public void HurtEnd()
    {
        manager.CombatManager.OnHurtAnimationComplete();
        manager.Animator.ResetTrigger(HURT);
        //Debug.Log("HurtEnd animation event called");
    }

    public void AttackEnd()
    {
        manager.Animator.ResetTrigger(Attack);
        manager.Animator.ResetTrigger(AOEAttack);
        manager.AttackManager.OnAnimationComplete();
        //Debug.Log("AttackEnd animation event called");
    }

    public void KnockbackEnd()
    {
        manager.Animator.ResetTrigger(KNOCKBACK);
        manager.Animator.speed = 1;

        // Let KnockbackManager handle the reset
        if (manager.KnockbackManager != null)
            manager.KnockbackManager.EndKnockback();

        //Debug.Log("KnockbackEnd animation event called");
    }
}