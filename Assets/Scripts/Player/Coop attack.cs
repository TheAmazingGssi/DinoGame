using System.Collections;
using UnityEngine;

public class CoopAttack : MonoBehaviour
{
    [SerializeField] private LegBurstSpawner legSpawner;
    [SerializeField] private ParticleSystem SmokeEffect;
    [SerializeField] private float smokeDuration = 2f;


    public void Activate()
    {
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
