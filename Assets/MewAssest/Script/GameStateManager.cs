using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject playerPrefeb;
    public Transform spawnPos;
    public float playerSpawnTime = 5f;
    public bool isPlayerDead;
    private float nextSpawnTime;
    private bool isSpawnTimeSet;

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
    }
}
