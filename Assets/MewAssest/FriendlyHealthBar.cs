using UnityEngine;
using UnityEngine.UI;

public class FriendlyHealthBar : MonoBehaviour
{
    public Transform gameObjectPos;
    private GameObject healthBar;
    private Image healthBarImage;
    private FriendlyController friendlyController;
    void Awake()
    {
        healthBar = transform.GetChild(0).gameObject;
        healthBarImage = transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Image>();
    }
    void Start()
    {
        gameObjectPos = transform.parent.transform;
        friendlyController = transform.parent.GetComponent<FriendlyController>();
        healthBar.SetActive(false);
    }
    void Update()
    {
        if(friendlyController.currentHealth < friendlyController.maxHealth)
        {
            healthBar.SetActive(true);
            healthBarImage.fillAmount = friendlyController.currentHealth / friendlyController.maxHealth;
            healthBar.transform.position = new Vector2 (gameObjectPos.position.x, gameObjectPos.position.y + 0.8f);
        }
        else
        {
            healthBar.SetActive(false);
        }
    }
}
