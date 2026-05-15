using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class BuildingClass : MonoBehaviour
{
    public int GoldRequire { get; protected set; }
    public int FoodRequire { get; protected set; }



    [field: SerializeField] protected GameObject HealthBar;

    public UnityEngine.UI.Image healthBarImage;
    public Canvas HealthCanvas;
    public int ProduceAmount { get; protected set; }
    public bool IsBuilt { get; protected set; }

    private float currentHealth;
    public float MaxHealth { get; protected set; }

    private void Awake()
    {

        HealthBar.transform.position = new Vector2(transform.position.x, transform.position.y + 0.8f);
        gameObject.SetActive(false);
        HealthBar.SetActive(true);
        
    }

    private void Update()
    {
        if (this.IsBuilt)
        {
            //HealthCanvas.transform.position = gameObject.transform.position;
            HealthCanvas.transform.rotation = Quaternion.identity;
        }
        if (this.currentHealth < this.MaxHealth)
        {
            HealthBar.SetActive(true);
        }
        else
        {
            HealthBar.SetActive(false);
        }

        healthBarImage.fillAmount = currentHealth / MaxHealth;
    }

    public void Init(int goldRequire, int foodRequire, int produceAmount, bool isBuilt, float maxHealth)
    {
        GoldRequire = goldRequire;
        FoodRequire = foodRequire;
        ProduceAmount = produceAmount;
        IsBuilt = isBuilt;
        MaxHealth = maxHealth;
        currentHealth = MaxHealth;
    }

    public void Build()
    {
        currentHealth = MaxHealth;
        if (this.IsBuilt) return;
        if (Resource.GetInstance().Gold < GoldRequire || Resource.GetInstance().Food < FoodRequire) return;
        Resource.GetInstance().Gold -= GoldRequire;
        Resource.GetInstance().Food -= FoodRequire;
        gameObject.SetActive(true);
        this.IsBuilt = true;
    }

    public virtual void Output()
    {
        if (this.IsBuilt)
        {
           Resource.GetInstance().Gold += ProduceAmount;
        }
    }

    public void Repair()
    {
        
        currentHealth += 50;
        Debug.Log("Repairing... Current Health: " + currentHealth);

    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            this.IsBuilt = false;

        }
    }

    


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.TryGetComponent (out EnemyController enemyController))
        {
            TakeDamage(enemyController.damage);

            enemyController.isHasHit = true;
            var dir = (gameObject.transform.position - enemyController.transform.position).normalized;
            enemyController.OnEnemyHit(0, dir, gameObject.transform ,0.5f);
        }

    }

    

}
