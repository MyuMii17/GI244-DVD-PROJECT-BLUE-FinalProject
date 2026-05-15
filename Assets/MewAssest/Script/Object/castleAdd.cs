using UnityEngine;

public class castleAdd : MonoBehaviour
{
    void OnEnable()
    {
        ObjectManager.castle.Add(gameObject.transform);
    }
    void OnDisable()
    {
        ObjectManager.castle.Remove(gameObject.transform);
    }
}
