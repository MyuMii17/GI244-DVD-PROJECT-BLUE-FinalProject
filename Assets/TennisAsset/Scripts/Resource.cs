using UnityEngine;

public class Resource : MonoBehaviour
{
    public static Resource ResourceInstance;

    public int Gold;
    public int Food;

    public int Citizen;

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

        DontDestroyOnLoad(this.gameObject);

        ResourceInstance = this;
    }

   

}
