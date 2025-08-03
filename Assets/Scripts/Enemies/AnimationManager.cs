using UnityEngine;

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

    //Animation events on last frame
    public void HurtEnd()
    {
        // Call the combat manager's method to handle color reset and state
        manager.CombatManager.OnHurtAnimationComplete();
        manager.Animator.ResetTrigger(HURT);
        Debug.Log("HurtEnd animation event called");
    }

    public void AttackEnd()
    {
        manager.Animator.ResetTrigger(Attack);
        manager.Animator.ResetTrigger(AOEAttack);
        manager.AttackManager.OnAnimationComplete();
        Debug.Log("AttackEnd animation event called");
    }

    public void KnockbackEnd()
    {
        manager.Animator.ResetTrigger(KNOCKBACK);
        manager.CombatManager.IsKnockbacked = false;
        manager.Animator.speed = 1; // Resume normal animation speed
        Debug.Log("KnockbackEnd animation event called");
    }
}