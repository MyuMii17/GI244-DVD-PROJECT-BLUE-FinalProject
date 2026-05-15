using UnityEngine;

public class ApothecaryAdd : MonoBehaviour
{
    private void OnEnable()
    {
        ObjectManager.apothecary.Add(transform);
    }

    private void OnDisable()
    {
        ObjectManager.apothecary.Remove(transform);
    }

}
