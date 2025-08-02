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
        Debug.Log("projectile launched");

        bool isFacingLeft = manager.EnemyController.IsFacingLeft();
        direction = isFacingLeft ? Vector3.left : Vector3.right;

        direction.y = 0;

        damage = manager.EnemyData.BaseDamage;
    }

    private void FixedUpdate()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER))
        {
            combatManager = collision.GetComponent<PlayerCombatManager>();
            if (combatManager)
            {
                combatManager.TakeDamage(new DamageArgs { Damage = damage });
                Destroy(gameObject);
            }
        }
    }
}
