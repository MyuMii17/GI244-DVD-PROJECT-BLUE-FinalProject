using UnityEngine;

public abstract class BuildingClass : MonoBehaviour
{
    public int GoldRequire { get; protected set; }
    public int FoodRequire { get; protected set; }

    public bool IsBuilt { get; protected set; }


    public void Init(int goldRequire, int foodRequire, bool isBuilt)
    {
        GoldRequire = goldRequire;
        FoodRequire = foodRequire;
        IsBuilt = isBuilt;
    }

    public void Build()
    {
        if (this.IsBuilt) return;
        gameObject.SetActive(true);
        this.IsBuilt = true;
    }

    
}
