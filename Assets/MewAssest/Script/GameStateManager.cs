using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject playerPrefeb;
    public Transform spawnPos;
    public float playerSpawnTime = 5f;
    public bool isPlayerDead;
    private float nextSpawnTime;
    private bool isSpawnTimeSet;
    public bool isGameOver;

    public List<GameObject> userInterfaces = new List<GameObject>();
    private static GameStateManager StaticInstance = null;
    public static GameStateManager GetStatic()
    {
        return StaticInstance;
    }

    void Awake()
    {
        if(StaticInstance != null)
        {
            Destroy(this);
            return;
        }

        StaticInstance = this;
    } 

    void Update()
    {
        var time = Time.time;
        if(PlayerManager.players.Count == 0)
        {
            if(isSpawnTimeSet == false)
            {
                isSpawnTimeSet = true;
                nextSpawnTime = time + playerSpawnTime;
            }

            if(isSpawnTimeSet == true && time >= nextSpawnTime)
            {
                Instantiate(
                    playerPrefeb,
                    spawnPos.position,
                    spawnPos.rotation
                );
                
                nextSpawnTime = time + playerSpawnTime;
                isSpawnTimeSet = true;
            }
        }
        else
        {
            isSpawnTimeSet = false;
        }

        if(ObjectManager.objects.Count == 0)
        {
            isGameOver = true;
        }
    }

    public void DisableUI()
    {
        foreach(var ui in userInterfaces)
        {
            ui.SetActive(false);
        }
    }

    public void GameOver()
    {
        
    }

    public void Resume()
    {
        
    }

    public void Setting()
    {
        
    }

    public void MainMenu()
    {
        
    }
}
