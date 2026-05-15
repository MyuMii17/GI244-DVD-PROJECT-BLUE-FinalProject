using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform gameObjectPos;
    private GameObject healthBar;
    private Image healthBarImage;
    private EnemyController enemyController;
    void Awake()
    {
        healthBar = transform.GetChild(0).gameObject;
        healthBarImage = transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Image>();
    }
    void Start()
    {
        gameObjectPos = transform.parent.transform;
        enemyController = transform.parent.GetComponent<EnemyController>();
        healthBar.SetActive(false);
    }
    void Update()
    {
        if(enemyController.currentHealth < enemyController.maxHealth)
        {
            healthBar.SetActive(true);
            healthBarImage.fillAmount = enemyController.currentHealth / enemyController.maxHealth;
            healthBar.transform.position = new Vector2 (gameObjectPos.position.x, gameObjectPos.position.y + 0.8f);
            healthBar.transform.rotation = Quaternion.identity;
        }
        else
        {
            healthBar.SetActive(false);
        }
    }
}
