using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public EnemyManager manager;
    public float speed;

    private const string PLAYER = "Player";

    private PlayerCombatManager combatManager;

    private Vector3 direction;
    private float timer = 3;
    private float damage;

    private void Start()
    {
        Debug.Log("projectile lunched");

        Vector3 targetPosition = manager.AttackManager.CurrentTarget.transform.position;
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
            Debug.Log("projectile found player");

            combatManager = other.GetComponent<PlayerCombatManager>();
            combatManager.TakeDamage(new DamageArgs { Damage = damage});
            Destroy(gameObject);
        }
    }
}
