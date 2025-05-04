using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class EnemyManager : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private EnemyCombatManager combatManager;
    [SerializeField] private EnemyController movement;


    public event UnityAction<EnemyManager> OnDeath;


    private void Start()
    {
        combatManager.OnDeath += HandleDeath;
    }


    private void OnEnable()
    {
        combatManager.Initialize();
    }

    private void HandleDeath(CombatManager combatManager)
    {
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (!combatManager)
        {
            combatManager = gameObject.GetComponentInChildren<EnemyCombatManager>();
        }
    }

}