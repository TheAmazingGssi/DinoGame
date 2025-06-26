using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private EnemyManager manager;
    [SerializeField] private float speed;

    private const string PLAYER = "Player";

    private PlayerCombatManager combatManager;

    private Vector3 direction;
    private float timer = 3;
    private float damage;

    private void Start()
    {
        Vector3 targetPosition = manager.CurrentTarget.transform.position;
        Vector3 startPosition = transform.position;

        direction = (targetPosition - startPosition).normalized;

        damage = manager.EnemyData.BaseDamage;
    }

    private void FixedUpdate()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;

        timer -= Time.fixedDeltaTime;

        if(timer <= 0 ) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(PLAYER))
        {
            combatManager = other.GetComponent<PlayerCombatManager>();
            combatManager.TakeDamage(new DamageArgs { Damage = damage});
            Destroy(gameObject);
        }
    }
}
