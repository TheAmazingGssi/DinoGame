using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static readonly int HURT = Animator.StringToHash("Hurt");
    private static readonly int KNOCKBACK = Animator.StringToHash("Knockback");


    [SerializeField] private EnemyManager manager;

    private void Update()
    {
        if (!manager.CombatManager.IsKnockbacked) manager.Animator.speed = 1;
    }

    public void HurtEnd()
    {
        manager.SpriteRenderer.color = Color.white;
        if (!manager.CombatManager.IsKnockbacked) manager.Animator.ResetTrigger(HURT);
    }
    public void KnockbackEnd()
    {
       manager.Animator.ResetTrigger(KNOCKBACK);
    }

    public void KnockbackPause()
    {
        manager.Animator.speed = 0;
        manager.CombatManager.IsKnockbacked = false;
    }
}
