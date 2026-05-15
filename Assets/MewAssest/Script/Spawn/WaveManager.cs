using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    private static WaveManager StaticInstance = null;
    private GameStateManager gameStateManager;
    [SerializeField]private SpawnManager spawnManager;
    public float currentDelay; 
    private int currentWave = 0;
    public List<Wave> waves = new List<Wave>();
    public static WaveManager GetStatic()
    {
        return StaticInstance;
    }
    void Awake()
    {
        if(StaticInstance != null)
        {
            Destroy(this);
        }

        StaticInstance = this;

        currentWave = 0;
        currentDelay = waves[currentWave].DelayStart;
        StartCoroutine(DelayWave());
    }

    void Start()
    {
        gameStateManager = GameStateManager.GetStatic();  
        gameStateManager.delay = currentDelay;  
        gameStateManager.wave = currentWave;
    }

    void Update()
    {
        if (currentWave >= waves.Count)
        {
            Time.timeScale = 0;
        }
    }
    IEnumerator DelayWave()
    {
        while (currentWave < waves.Count)
        {
            yield return new WaitForSeconds(waves[currentWave].DelayStart);

            gameStateManager.wave = currentWave;
            spawnManager.ChangeWave(waves[currentWave]);

            yield return new WaitUntil(() => spawnManager.IsCompleted());
            currentWave++;

            if (currentWave >= waves.Count)
            {
                gameStateManager.isWin = true;
                yield break;
            }

            yield return null;

            gameStateManager.isWaveStart = false;
            currentDelay = waves[currentWave].DelayStart;
            gameStateManager.delay = currentDelay;
        }
        
    }
}
