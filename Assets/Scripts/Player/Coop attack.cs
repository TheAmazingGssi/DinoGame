using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopAttack : MonoBehaviour
{
    [SerializeField] private LegBurstSpawner legSpawner;
    [SerializeField] private ParticleSystem SmokeEffect;
    [SerializeField] private float smokeDuration = 2f;


    public void Activate()
    {
        foreach (var enemy in new List<EnemyManager>(GameManager.Instance.ActiveEnemies))
        {
            enemy.CombatManager.TakeDamage(new DamageArgs(1000));
        }
        StartCoroutine(EffectCoroutine());
    }


    private IEnumerator EffectCoroutine()
    {
        SmokeEffect.Play();
        legSpawner.TriggerBurst();
        yield return new WaitForSeconds(smokeDuration);
        SmokeEffect.Stop();
    }
}
