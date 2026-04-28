using UnityEngine;

public abstract class BuildingClass : MonoBehaviour
{
    public int GoldRequire { get; protected set; }
    public int FoodRequire { get; protected set; }

    public int ProduceAmount { get; protected set; }
    public bool IsBuilt { get; protected set; }

    private void Awake()
    {
        gameObject.SetActive(false);
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
}
