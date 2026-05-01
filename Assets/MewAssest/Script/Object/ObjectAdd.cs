using System.Collections.Generic;
using UnityEngine;

public class ObjectAdd : MonoBehaviour
{
    void OnEnable()
    {
        objectManafer.objects.Add(gameObject.transform);
    }
    void OnDisable()
    {
        objectManafer.objects.Remove(gameObject.transform);
    }
}
