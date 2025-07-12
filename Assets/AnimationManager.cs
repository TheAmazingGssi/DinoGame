using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static readonly int Hurt = Animator.StringToHash("Hurt");


    [SerializeField] private EnemyManager manager;

    public void HurtEnd()
    {
        manager.SpriteRenderer.color = Color.white;
        manager.Animator.ResetTrigger(Hurt);
    }
}
