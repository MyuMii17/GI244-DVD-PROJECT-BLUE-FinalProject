using System.Collections.Generic;
using UnityEngine;

public class ObjectAdd : MonoBehaviour
{

    void OnEnable()
    {
        ObjectManager.objects.Add(gameObject.transform);
    }
    void OnDisable()
    {
        ObjectManager.objects.Remove(gameObject.transform);
    }
}
