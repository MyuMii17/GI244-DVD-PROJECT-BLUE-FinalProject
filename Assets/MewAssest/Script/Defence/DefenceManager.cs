using System.Collections.Generic;
using UnityEngine;

public class DefenceManager : MonoBehaviour
{
    public List<Transform> defNormalPositions = new List<Transform>();
    public List<Transform> hasSelectDefNormal = new List<Transform>();
    public List<Transform> defRangePositions = new List<Transform>();
    public List<Transform> hasSelectDefRange = new List<Transform>();
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
