using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject playerPrefeb;
    public Transform spawnPos;

    public GameObject delayPanel;
    public TMP_Text delayCount;
    public GameObject wavePanel;
    public TMP_Text waveCount;
    public GameObject skillCoolDownPanel;
    public TMP_Text skillCoolDown;
    public float playerSpawnTime = 5f;
    public float skillCoolDownCount;
    public float wave;
    public float delay;
    public bool isPlayerDead;
    private float nextSpawnTime;
    private bool isSpawnTimeSet;
    public bool isGameOver;
    public bool isWaveStart;
    public bool isSkillCooldown;

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

    void Start()
    {
        isWaveStart = false;
        Wave();
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

        if (!isWaveStart && delay > 0)
        {
            delayPanel.SetActive(true);
            delay -= Time.deltaTime;

            int delayInt = (int)delay;

            delayCount.text = delayInt.ToString();
        }
        else
        {
            StartCoroutine(WaveStart());
        }

        if (isSkillCooldown && skillCoolDownCount >= 0)
        {
            skillCoolDownPanel.SetActive(true);
            skillCoolDownCount -= Time.deltaTime;

            int skillInt = (int)skillCoolDownCount;

            skillCoolDown.text = $"{skillInt} Ricochet";
        }
        else
        {
            skillCoolDownPanel.SetActive(false);
        }
    }

    IEnumerator WaveStart()
    {
        if(isWaveStart == true) yield break;
        isWaveStart = true;

        Wave();

        delayCount.text = "Wave Start!!";

        yield return new WaitForSeconds(1);

        delayPanel.SetActive(false);
    }

    public void Wave()
    {
        waveCount.text = $"Wave {wave}";
    }

    public void DisableUI()
    {
        Time.timeScale = 1;
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
