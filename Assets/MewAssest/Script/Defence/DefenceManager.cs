using System.Collections.Generic;
using UnityEngine;

public class DefenceManager : MonoBehaviour
{
    public List<GameObject> defNormalPositions = new List<GameObject>();
    public List<GameObject> hasSelectDefNormal = new List<GameObject>();
    public List<GameObject> defRangePositions = new List<GameObject>();
    public List<GameObject> hasSelectDefRange = new List<GameObject>();
    private static DefenceManager StaticInstance = null;
    public static DefenceManager GetStatic()
    {
        return StaticInstance;
    }
    void Awake()
    {
        if(StaticInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        StaticInstance = this;
    }
}
