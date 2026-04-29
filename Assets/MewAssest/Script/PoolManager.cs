using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static List<GameObject> ArrowPrefeb = new List<GameObject>();
    private static PoolManager staticInstance;
    public static PoolManager GetStatic()
    {
        return staticInstance;
    }
    void Awake()
    {
        if(staticInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        staticInstance = this;
    }
}
