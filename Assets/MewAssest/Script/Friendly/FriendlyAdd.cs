using UnityEngine;

public class FriendlyAdd : MonoBehaviour
{
    void OnEnable()
    {
        FriendlyManager.friendlys.Add(transform);
    }
    void OnDisable()
    {
        FriendlyManager.friendlys.Remove(transform);
    }
}
