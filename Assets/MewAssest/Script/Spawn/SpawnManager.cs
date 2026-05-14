using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private Wave wave;
    private static SpawnManager StaticInstance = null;
    public List<Transform> spawnPoint = new List<Transform>();
    public GameObject normalEnemies;
    public GameObject rangeEnemies;
    [SerializeField]private WaveManager waveManager;
    public List<Transform> spawnPointSelects;
    public bool canSpawnEnemies;
    public bool isStart;
    public int normalEnemy;
    public int rangeEnemy;
    public int totalEnemies;
    public int enemiesDead;
    public float nextSpawnTime;
    public int waveCount = 0;
    private Coroutine spawnCoroutine;
    public bool isCanSpawn;
    public static SpawnManager GetStatic()
    {
        return StaticInstance;
    }
    void Awake()
    {
        if(StaticInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        StaticInstance = this;

    }
    public void ChangeWave(Wave wave)
    {
        this.wave = wave;
        canSpawnEnemies = true;

        isStart = true;
        totalEnemies = 0;
        normalEnemy = 0;
        rangeEnemy = 0;

        enemiesDead = 0;

        SelectsPoint(wave.numberOfRandomSpawnPoint);
        StartCoroutine(SpawnEnemy());
    }

    public bool IsCompleted()
    {
        return enemiesDead >= wave.totalEnemySpawn;
    }

    void Update()
    {
        if (isStart)
        {
            if (totalEnemies < wave.totalEnemySpawn)
            {
                canSpawnEnemies = true;
            }
            else
            {
                waveCount++;
                canSpawnEnemies = false;
            }
        }
    }
    void SelectsPoint(int amount)
    {
        spawnPointSelects = new List<Transform>(spawnPoint);
        for(int i = 0; i < spawnPointSelects.Count; i++)
        {
            int rand = Random.Range(i, spawnPointSelects.Count);
            Transform t = spawnPointSelects[i];
            spawnPointSelects[i] = spawnPointSelects[rand];
            spawnPointSelects[rand] = t;
        }
        spawnPointSelects = spawnPointSelects.GetRange(0, amount);
    }

    IEnumerator SpawnEnemy()
    {
        while (canSpawnEnemies)
        {
            if(normalEnemy < wave.normalEnemySpawn)
            {
                var point = Random.Range(0, wave.numberOfRandomSpawnPoint);
                Instantiate(
                    normalEnemies, 
                    spawnPointSelects[point].position, 
                    Quaternion.identity
                );
                totalEnemies++;
                normalEnemy++;
                yield return new WaitForSeconds(wave.normalSpawnInterval);
            }

            if(rangeEnemy < wave.rangeEnemySpawn)
            {
                var point = Random.Range(0, wave.numberOfRandomSpawnPoint);
                Instantiate(
                    rangeEnemies, 
                    spawnPointSelects[point].position, 
                    Quaternion.identity
                );
                totalEnemies++;
                rangeEnemy++;
                yield return new WaitForSeconds(wave.rangeEnemySpawn);
            }
        }
    }
}
