using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager StaticInstance = null;
    private List<Transform> spawnPos = new List<Transform>();
    public List<GameObject> enemiesPerfebs = new List<GameObject>();
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
        DontDestroyOnLoad(gameObject);

        StaticInstance = this;

        for(int i = 0; i < 4; i++)
        {
            spawnPos.Add(transform.GetChild(i).transform);
        }
        isCanSpawn = true;
    }
    void Update()
    {
        if(isCanSpawn == true)
        {
            if(spawnCoroutine != null) return;
            spawnCoroutine = StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {
        isCanSpawn = false;
        int spawnIndex = Random.Range(0,4);
        Instantiate(
            enemiesPerfebs[0], 
            spawnPos[spawnIndex].position, 
            Quaternion.identity
        );
        
        yield return new WaitForSeconds(1);
        spawnCoroutine = null;
        isCanSpawn = true;
    }
}
