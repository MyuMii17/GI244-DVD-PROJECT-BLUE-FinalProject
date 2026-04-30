using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Transform playerPos;
    private GameObject healthBar;
    private Image healthBarImage;
    private PlayerController playerController;
    void Awake()
    {
        healthBar = transform.GetChild(0).gameObject;
        healthBarImage = transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Image>();
    }
    void Start()
    {
        playerController = PlayerController.GetStatic();
        healthBar.SetActive(false);
    }
    void Update()
    {
        if(playerController.currentHealth < playerController.maxHealth)
        {
            healthBar.SetActive(true);
            healthBarImage.fillAmount = playerController.currentHealth / playerController.maxHealth;
            healthBar.transform.position = new Vector2 (playerPos.position.x,playerPos.position.y + 0.8f);
        }
        else
        {
            healthBar.SetActive(false);
        }
    }
}
