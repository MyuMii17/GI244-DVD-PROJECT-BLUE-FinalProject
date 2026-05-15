using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject speedDownPanel;
    public TMP_Text speedDownCountText;
    public TMP_Text speedDown;
    public GameObject speedUpPanel;
    public TMP_Text speedUpCountText;
    public TMP_Text speedUp;
    public GameObject settingPanel;
    public Slider musicSlider;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public float speedDownCounts;
    public float speedDownTime;
    public bool isSpeedDownCooldown;
     public float speedUpCounts;
    public float speedUpTime;
    public bool isSpeedUpCooldown;
    public float playerSpawnTime = 5f;
    public float skillCoolDownCount;
    public float wave;
    public float delay;
    public bool isPlayerDead;
    public bool isGamePause;
    public bool isSettingOpen;
    private float nextSpawnTime;
    private bool isSpawnTimeSet;
    public bool isGameOver;
    public bool isWin;
    public bool isWaveStart;
    public bool isSkillCooldown;

    private int mainMenuIndex = 0;
    private int gameIndex = 1;
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

        gameOverPanel.SetActive(false);
    }

    void Start()
    {
        isWaveStart = false;
        Wave();
    }

    void Update()
    {
        var time = Time.time;

        if(Time.timeScale == 0)
        {
            isGamePause = true;
        }
        else
        {
            isGamePause = false;
        }

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

        if (isGameOver)
        {
            Pause();
            gameOverPanel.SetActive(true);
        }

        if (isWin)
        {
            Pause();
            winPanel.SetActive(true);
        }

        if (isSpeedDownCooldown && speedDownTime >= 0)
        {
            speedDownPanel.SetActive(true);
            speedDownTime -= Time.deltaTime;

            int speedDownInt = (int)speedDownTime;

            speedDown.text = $"{speedDownInt} SpeedDown";
        }
        else
        {
            isSpeedDownCooldown = false;
            speedDownPanel.SetActive(false);
        }

        if (isSpeedUpCooldown && speedUpTime >= 0)
        {
            speedUpPanel.SetActive(true);
            speedUpTime -= Time.deltaTime;

            int speedUpInt = (int)speedUpTime;

            speedUp.text = $"{speedUpInt} SpeedUp";
        }
        else
        {
            isSpeedUpCooldown = false;
            speedUpPanel.SetActive(false);
        }

        speedDownCountText.text = speedDownCounts.ToString();
        speedUpCountText.text = speedUpCounts.ToString();
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
    public void Pause()
    {
        DisableUI();
        Time.timeScale = 0;
        isGamePause = true;
    }
    public void Resume()
    {
        DisableUI();
        Time.timeScale = 1;
        isGamePause = false;
    }

    void DisableUI()
    {
        foreach(var ui in userInterfaces)
        {
            ui.SetActive(false);
        }
    }

    public void Restart()
    {
        Resume();
        SceneManager.LoadScene(gameIndex);
    }

    public void Setting()
    {
        isSettingOpen = true;
        settingPanel.SetActive(true);
    }
    public void ReturnToPauseMenu()
    {
        isSettingOpen = false;
        settingPanel.SetActive(false);
    }

    public void MainMenu()
    {
        Resume();
        SceneManager.LoadScene(mainMenuIndex);
    }
}
