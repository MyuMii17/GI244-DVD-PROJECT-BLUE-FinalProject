using UnityEngine;

public class Resource : MonoBehaviour
{
    public static Resource ResourceInstance;

    public int Gold;
    public int Food;
    public int Citizen;
    public int MaxCitizen = 30;
    public static Resource GetInstance()
    {
        return ResourceInstance;
    }

    private void Awake()
    {
        if(ResourceInstance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        ResourceInstance = this;

        MaxCitizen = 30;
    }

    private void Update()
    {
        if (Citizen > MaxCitizen)
        { 
            Citizen = MaxCitizen;
        }

    }


}
