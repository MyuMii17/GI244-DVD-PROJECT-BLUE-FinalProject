using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [SerializeField]private SpawnManager spawnManager;
    private int currentWave = 0;
    public List<Wave> waves = new List<Wave>();
    public float currentDelay; 
    void Awake()
    {
        currentWave = 0;
        currentDelay = waves[currentWave].DelayStart;
        StartCoroutine(DelayWave());
    }
    void Update()
    {
        if (currentWave > 2)
        {
            Time.timeScale = 0;
        }
    }
    IEnumerator DelayWave()
    {
        while (currentWave < waves.Count)
        {
            yield return new WaitForSeconds(waves[currentWave].DelayStart);

            spawnManager.ChangeWave(waves[currentWave]);

            yield return new WaitUntil(() => spawnManager.IsCompleted());

            currentWave++;
            currentDelay = waves[currentWave].DelayStart;

            yield return null;

            if (currentWave >= waves.Count)
            {
                yield break;
            }
        }
        
    }
}
