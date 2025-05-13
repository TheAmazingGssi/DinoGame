using System.Collections.Generic;
using UnityEngine;


public class EnemyPool : MonoBehaviour
{
    [SerializeField] private List<EnemyManager> EnemyPrefabs;

    [SerializeField] private int AmountToPool = 10;

    private List<EnemyManager> enemyPool = new List<EnemyManager>();

    private float timer = 0;

    public float Timer => timer;

    private int lastEnemyIndex = 0;

    private int maxEnemyTypes { get { return (int)Mathf.Clamp(timer / 30, 0, EnemyPrefabs.Count); } }

    private void Update()
    {
        timer += Time.deltaTime;

        if (maxEnemyTypes + 1 > lastEnemyIndex && lastEnemyIndex < EnemyPrefabs.Count)
        {
            AddEnemyToPool(lastEnemyIndex++);
        }
    }

    public void InitializePool()
    {
        AddEnemyToPool(lastEnemyIndex++);
    }

    public EnemyManager GetEnemy()
    {
        if (enemyPool.Count > 0)
        {
            EnemyManager enemyToReturn = enemyPool[Random.Range(0, enemyPool.Count)];
            enemyPool.Remove(enemyToReturn);
            return enemyToReturn;
        }
        return Instantiate(EnemyPrefabs[Random.Range(0, enemyPool.Count)], transform);
    }

    public void ReleaseEnemy(EnemyManager enemy)
    {
        enemy.gameObject.SetActive(false);
        enemyPool.Add(enemy);
    }
    private void AddEnemyToPool(int enemyIndex)
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            EnemyManager currentEnemy = Instantiate(EnemyPrefabs[enemyIndex], transform);
            currentEnemy.gameObject.SetActive(false);
            enemyPool.Add(currentEnemy);
        }
    }
}