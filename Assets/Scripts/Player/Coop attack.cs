using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopAttack : MonoBehaviour
{
    [SerializeField] private LegBurstSpawner legSpawner;
    [SerializeField] private ParticleSystem SmokeEffect;
    
    [SerializeField] private float smokeDuration = 5f;
    [SerializeField] private float effectStartDelay = 0.75f;
    [SerializeField] private float damageDelay = 0.5f;
    
    [SerializeField] private Rigidbody2D meteorFlareRB;
    [SerializeField] private float flareSpeed = 5f;
    [SerializeField] private Transform flareSpawnPosition;
    
    [SerializeField] private float bossDmg = 200f;
    [SerializeField] private float normEnemyDmg = 1000f;

    public void Activate()
    {
        StartCoroutine(CoopAttackSequance());
    }

    private IEnumerator CoopAttackSequance()
    {
        GameManager.Instance.IsInCoopAttack = true;
        meteorFlareRB.transform.position = flareSpawnPosition.position;
        
        meteorFlareRB.gameObject.SetActive(true);
        meteorFlareRB.linearVelocity = Vector2.up * flareSpeed;
        
        yield return new WaitForSeconds(effectStartDelay);
        
        meteorFlareRB.gameObject.SetActive(false);
        meteorFlareRB.transform.position = flareSpawnPosition.position;
        
        StartCoroutine(EffectCoroutine());
        
        yield return new WaitForSeconds(damageDelay);
        
        foreach (var enemy in new List<EnemyManager>(GameManager.Instance.ActiveEnemies))
        {
            if(enemy.EnemyData.Type == EnemyType.Boss) 
                enemy.CombatManager.TakeDamage(new DamageArgs(bossDmg));
            else if (enemy && enemy?.CombatManager) 
                enemy.CombatManager.TakeDamage(new DamageArgs(normEnemyDmg));
        }
        
        yield return new WaitForSeconds(4.5f);
        GameManager.Instance.IsInCoopAttack = false;
    }


    private IEnumerator EffectCoroutine()
    {
        SmokeEffect.Play();
        legSpawner.TriggerBurst();
        yield return new WaitForSeconds(smokeDuration);
        SmokeEffect.Stop();
    }
}
