using UnityEngine;

public class BossVFXManager : MonoBehaviour
{
    [Header("Boss VFX Animators")]
    public Animator AttackVfxAnimator;
    public Animator AOEAttackVfxAnimator;
    public Animator[] HurtVfxAnimator;
    public Animator DeathVfxAnimator;

    private bool attackVfxPlaying = false;
    private bool aoeVfxPlaying = false;
    private bool hurtVfxPlaying = false;
    private bool deathVfxPlaying = false;

    private void Awake()
    {
        AttackVfxAnimator.gameObject.SetActive(false);
        AOEAttackVfxAnimator.gameObject.SetActive(false);
        for (int i = 0; i < HurtVfxAnimator.Length; i++)
            HurtVfxAnimator[i].gameObject.SetActive(false);
       // DeathVfxAnimator.gameObject.SetActive(false);
    }

    public void TriggerAttackVfx()
    {
        if (attackVfxPlaying || AttackVfxAnimator == null) return;
        attackVfxPlaying = true;
        AttackVfxAnimator.gameObject.SetActive(true);
        AttackVfxAnimator.SetTrigger("Play");
    }

    public void TriggerAOEVfx()
    {
        if (aoeVfxPlaying || AOEAttackVfxAnimator == null) return;
        aoeVfxPlaying = true;
        AOEAttackVfxAnimator.gameObject.SetActive(true);
        AOEAttackVfxAnimator.SetTrigger("Play");
    }

    public void TriggerHurtVfx()
    {
        if (hurtVfxPlaying || HurtVfxAnimator == null) return;

        hurtVfxPlaying = true;
        for (int i = 0; i < HurtVfxAnimator.Length; i++)
        {
            HurtVfxAnimator[i].gameObject.SetActive(true);
            HurtVfxAnimator[i].SetTrigger("Play");
        }
    }

    public void TriggerDeathVfx()
    {
        if (deathVfxPlaying || DeathVfxAnimator == null) return;
        Debug.Log($"AAAAAAAAAAAAAAAAA");

        deathVfxPlaying = true;
        DeathVfxAnimator.gameObject.SetActive(true);
        DeathVfxAnimator.SetTrigger("Play");
    }
    public void ResetAttackVfx() => attackVfxPlaying = false;
    public void ResetAOEVfx() => aoeVfxPlaying = false;
    public void ResetHurtVfx() => hurtVfxPlaying = false;
    public void ResetDeathVfx() => deathVfxPlaying = false;

}
