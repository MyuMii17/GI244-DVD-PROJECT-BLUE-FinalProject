using UnityEngine;
using UnityEngine.UI;

public abstract class BuildingClass : MonoBehaviour
{
    public int GoldRequire { get; protected set; }
    public int FoodRequire { get; protected set; }

    [field: SerializeField] protected GameObject HealthBar;
    private Image healthBarImage;
    public int ProduceAmount { get; protected set; }
    public bool IsBuilt { get; protected set; }

    private void Awake()
    {
        gameObject.SetActive(false);
        HealthBar.SetActive(false);
        HealthBar.transform.position = new Vector2(transform.position.x, transform.position.y + 0.8f);
    }

    public void Init(int goldRequire, int foodRequire, int produceAmount, bool isBuilt)
    {
        GoldRequire = goldRequire;
        FoodRequire = foodRequire;
        ProduceAmount = produceAmount;
        IsBuilt = isBuilt;
    }

    public void Build()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthBar.SetActive(true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        HealthBar.SetActive(false);
    }
}
